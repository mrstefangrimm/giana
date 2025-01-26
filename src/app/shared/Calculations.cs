using Giana.Api.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Giana.App.Shared;

public static class Calculations
{
  public static Routine CreateRoutine(this Query query, TextWriter outputWriter)
  {
    ArgumentNullException.ThrowIfNull(query.Sources);
    ArgumentNullException.ThrowIfNull(query.Analyzer);
    ArgumentNullException.ThrowIfNull(query.OutputFormat);
    ArgumentNullException.ThrowIfNull(outputWriter);

    var routine = new Routine();

    routine.Sources = [.. query.Sources];
    routine.CommitsFrom = query.CommitsFrom;

    routine.TimeRanges = new List<(Func<IImmutableList<GitLogRecord>, DateTime, DateTime, IImmutableList<GitLogRecord>> Invoke, DateTime Begin, DateTime End)>();
    foreach (var timePeriod in query.TimeRanges)
    {
      routine.TimeRanges.Add((Api.Core.Calculations.WithTimeRange, timePeriod.Begin, timePeriod.End));
    }

    routine.Renames = new List<(Func<IImmutableList<GitLogRecord>, string, string, IImmutableList<GitLogRecord>> Invoke, string To, string From)>();
    foreach (var authorRename in query.Renames)
    {
      routine.Renames.Add((Api.Core.Calculations.RenameAuthor, authorRename.To, authorRename.From));
    }

    routine.Reductions = new List<(Func<IImmutableList<GitLogRecord>, Regex, IImmutableList<GitLogRecord>> Invoke, Regex Argument)>();

    if (query.Includes.Names.Any())
    {
      var expr = string.Join('|', query.Includes.Names);
      routine.Reductions.Add((Api.Core.Calculations.IncludeName, new Regex(expr)));
    }
    if (query.Includes.Commits.Any())
    {
      var expr = string.Join('|', query.Includes.Commits);
      routine.Reductions.Add((Api.Core.Calculations.IncludeCommit, new Regex(expr)));
    }
    if (query.Includes.Authors.Any())
    {
      var expr = string.Join('|', query.Includes.Authors);
      routine.Reductions.Add((Api.Core.Calculations.IncludeAuthor, new Regex(expr)));
    }
    if (query.Includes.Messages.Any())
    {
      var expr = string.Join('|', query.Includes.Messages);
      routine.Reductions.Add((Api.Core.Calculations.IncludeMessage, new Regex(expr)));
    }

    foreach (var name in query.Excludes.Names)
    {
      routine.Reductions.Add((Api.Core.Calculations.ExcludeName, new Regex(name)));
    }
    foreach (var commit in query.Excludes.Commits)
    {
      routine.Reductions.Add((Api.Core.Calculations.ExcludeCommit, new Regex(commit)));
    }
    foreach (var author in query.Excludes.Authors)
    {
      routine.Reductions.Add((Api.Core.Calculations.ExcludeAuthor, new Regex(author)));
    }
    foreach (var msg in query.Excludes.Messages)
    {
      routine.Reductions.Add((Api.Core.Calculations.ExcludeMessage, new Regex(msg)));
    }

    routine.Analyze = query.Analyzer.ToLower() switch
    {
      "author-activity" => Api.Analysis.Activity.AuthorActivityActions.Execute,
      "file-coupling" => Api.Analysis.Coupling.FileCouplingActions.Execute,
      "folder-coupling-and-cohesion" => Api.Analysis.Coupling.FolderCouplingAndCohesionActions.Execute,
      "project-coupling-and-cohesion" => Api.Analysis.Coupling.ProjectCouplingAndCohesionActions.Execute,
      "author-ranking" => Api.Analysis.Ranking.AuthorRankingActions.Execute,
      "commit-ranking" => Api.Analysis.Ranking.CommitRankingActions.Execute,
      "file-ranking" => Api.Analysis.Ranking.FileRankingActions.Execute,
      _ => throw new NotImplementedException(query.Analyzer)
    };

    return routine;
  }

  public static string Name([CallerMemberName] string callingMethod = "")
  {
    return callingMethod;
  }
}
