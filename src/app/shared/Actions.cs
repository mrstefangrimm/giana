using Giana.Api.Core;
using Giana.Api.Load;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Giana.App.Shared.Calculations;

namespace Giana.App.Shared;

public static class Actions
{
  private static readonly object _lock = new object();

  public static async Task ExecuteAsync(Routine routine, Func<string> getGitExePath, int millisecondsTimeout)
  {
    using var cancellationSource = new CancellationTokenSource(millisecondsTimeout);

    ImmutableList<GitLogRecord> reducedRecords = [];
    ImmutableList<string> allActiveNames = [];

    try
    {
      await Parallel.ForEachAsync(routine.Sources, cancellationSource.Token, async (source, cancellationToken) =>
      {
        using var gitRepo = await GitRepository.CreateAsync(source, getGitExePath(), cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        var records = await gitRepo.LogAsync(cancellationToken, routine.CommitsFrom);

        records = records.Where(x => routine.TimeRanges.Count == 0 || routine.TimeRanges.Any(tp => tp.Begin <= x.Date && x.Date <= tp.End)).ToImmutableList();

        foreach (var renameAuthor in routine.Renames)
        {
          records = renameAuthor.Invoke(records, renameAuthor.To, renameAuthor.From);
        }

        foreach (var reduction in routine.Reductions)
        {
          records = reduction.Invoke(records, reduction.Argument);
        }

        lock (_lock)
        {
          reducedRecords = reducedRecords.AddRange(records);
        }

        // reducedNamesFromRecords can include historical items which are no longer active.
        var reducedNamesFromRecords = reducedRecords.Select(x => x.Name).Distinct();
        var reducedActiveNames = (await gitRepo.ActiveNamesAsync(cancellationToken)).Where(x => reducedNamesFromRecords.Contains(x));
        lock (_lock)
        {
          allActiveNames = allActiveNames.AddRange(reducedActiveNames);
        }
      });

      routine.Analyze(new Api.Analysis.ExecutionContext(reducedRecords, allActiveNames, routine.OutputFormat, routine.OutputWriter, cancellationSource.Token));
    }
    catch (OperationCanceledException)
    {
      Console.WriteLine($"`{Name()}` timeout of {millisecondsTimeout} ms reached. Ended without results.");
    }
  }
}
