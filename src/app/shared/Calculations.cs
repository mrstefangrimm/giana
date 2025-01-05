using Giana.Api.Shared;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace Giana.App.Shared;

public static class Calculations
{
  public static QueryRoutine CreateAction(this Query query)
  {
    var action = new QueryRoutine();

    action.Sources = [.. query.Sources];

    action.Reductions = new List<(Func<IEnumerable<GitLogRecord>, Regex, ImmutableList<GitLogRecord>> Invoke, Regex Argument)>();

    if (query.Includes.Names.Any())
    {
      var expr = string.Join('|', query.Includes.Names);
      action.Reductions.Add((Api.Shared.Calculations.IncludeName, new Regex(expr)));
    }
    if (query.Includes.Commits.Any())
    {
      var expr = string.Join('|', query.Includes.Commits);
      action.Reductions.Add((Api.Shared.Calculations.IncludeCommit, new Regex(expr)));
    }
    if (query.Includes.Authors.Any())
    {
      var expr = string.Join('|', query.Includes.Authors);
      action.Reductions.Add((Api.Shared.Calculations.IncludeAuthor, new Regex(expr)));
    }
    if (query.Includes.Messages.Any())
    {
      var expr = string.Join('|', query.Includes.Messages);
      action.Reductions.Add((Api.Shared.Calculations.IncludeMessage, new Regex(expr)));
    }

    foreach (var name in query.Excludes.Names)
    {
      action.Reductions.Add((Api.Shared.Calculations.ExcludeName, new Regex(name)));
    }
    foreach (var commit in query.Excludes.Commits)
    {
      action.Reductions.Add((Api.Shared.Calculations.ExcludeCommit, new Regex(commit)));
    }
    foreach (var author in query.Excludes.Authors)
    {
      action.Reductions.Add((Api.Shared.Calculations.ExcludeAuthor, new Regex(author)));
    }
    foreach (var msg in query.Excludes.Messages)
    {
      action.Reductions.Add((Api.Shared.Calculations.ExcludeMessage, new Regex(msg)));
    }

    action.Renames = new List<(Func<IEnumerable<GitLogRecord>, string, string, ImmutableList<GitLogRecord>> Invoke, string To, string From)>();
    foreach (var authorRename in query.Renames)
    {
      action.Renames.Add((Api.Shared.Calculations.RenameAuthor, authorRename.To, authorRename.From));
    }

    action.Analyze = query.Analyzer.ToLower() switch
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

    return action;
  }

}
