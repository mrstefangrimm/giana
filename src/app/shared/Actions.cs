using Giana.Api.Load;
using Giana.Api.Shared;
using Giana.Api.Shared.Fluent;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Giana.App.Shared;

public static class Actions
{
  public static ImmutableList<GitLogRecord> Execute(Query query)
  {
    ImmutableList<GitLogRecord> reducedRecords = [];

    foreach (var source in query.Sources)
    {
      using var gitRepo = GitRepository.Create(source, GitExePath());

      var records = gitRepo.Log();

      var renameBuilder = records.Rename();

      foreach (var renameAuthor in query.Renames)
      {
        renameBuilder = renameBuilder.And(renameAuthor.To, renameAuthor.From);
      }

      var includeBuilder = renameBuilder.Include();

      //foreach (var author in query.Includes.Authors)
      //{
      //  includeBuilder = includeBuilder.Author(author);
      //}

      reducedRecords = reducedRecords.AddRange(records);
    }

    //query.Analyzer(reducedRecords, query.OutputWriter);

    return reducedRecords;
  }

  public static ImmutableList<GitLogRecord> Execute(QueryRoutine query)
  {
    ImmutableList<GitLogRecord> reducedRecords = [];
    ImmutableList<string> allActiveNames = [];

    foreach (var source in query.Sources)
    {
      using var gitRepo = GitRepository.Create(source, GitExePath());

      var records = gitRepo.Log();

      foreach (var renameAuthor in query.Renames)
      {
        records = renameAuthor.Invoke(records, renameAuthor.To, renameAuthor.From);
      }

      foreach (var reduction in query.Reductions)
      {
        records = reduction.Invoke(records, reduction.Argument);
      }

      reducedRecords = reducedRecords.AddRange(records);

      var reducedNamesFromCommits = reducedRecords.Select(x => x.Name).Distinct();

      var reducedActiveNames = gitRepo.ActiveNames().Where(x => reducedNamesFromCommits. Contains(x));

      allActiveNames = allActiveNames.AddRange(reducedActiveNames);
    }

    query.Analyze(new Api.Analysis.ExecutionContext(reducedRecords, allActiveNames, query.OutputFormat, query.OutputWriter, new System.Threading.CancellationToken()));

    return reducedRecords;
  }

  private static string GitExePath()
  {
    return Environment.GetEnvironmentVariable("gitExePath");
  }
}
