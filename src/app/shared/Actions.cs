using Giana.Api.Core;
using Giana.Api.Load;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Giana.App.Shared;

public static class Actions
{
  public static ImmutableList<GitLogRecord> Execute(Routine routine, Func<string> getGitExePath)
  {
    ImmutableList<GitLogRecord> reducedRecords = [];
    ImmutableList<string> allActiveNames = [];

    foreach (var source in routine.Sources)
    {
      using var gitRepo = GitRepository.Create(source, getGitExePath());

      var records = gitRepo.Log(routine.Deadline);

      records = records.Where(x => routine.TimeRanges.Count == 0 || routine.TimeRanges.Any(tp => tp.Begin <= x.Date && x.Date <= tp.End)).ToImmutableList();

      foreach (var renameAuthor in routine.Renames)
      {
        records = renameAuthor.Invoke(records, renameAuthor.To, renameAuthor.From);
      }

      foreach (var reduction in routine.Reductions)
      {
        records = reduction.Invoke(records, reduction.Argument);
      }

      reducedRecords = reducedRecords.AddRange(records);

       // reducedNamesFromRecords can include historical items which are no longer active.
      var reducedNamesFromRecords = reducedRecords.Select(x => x.Name).Distinct();
      var reducedActiveNames = gitRepo.ActiveNames().Where(x => reducedNamesFromRecords. Contains(x));
      allActiveNames = allActiveNames.AddRange(reducedActiveNames);
    }

    routine.Analyze(new Api.Analysis.ExecutionContext(reducedRecords, allActiveNames, routine.OutputFormat, routine.OutputWriter, new System.Threading.CancellationToken()));

    return reducedRecords;
  }
}
