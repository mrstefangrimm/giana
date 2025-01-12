using Giana.Api.Core;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Giana.Api.Analysis.Ranking;

public static class CommitRankingCalculations
{
  public static IImmutableList<CommitRanking> CreateCommitRankingSorted(this IImmutableList<GitLogRecord> records)
  {
    var groupedByCommit = records.GroupBy(x => x.Commit);
    var commitRanking = groupedByCommit.Select(group =>
    {
      var commit = group.First();
      return new CommitRanking(commit.RepoName, commit.Commit, commit.Message, group.Count());
    });

    return commitRanking.ToImmutableList().Sort((a, b) => b.CommittedFiles - a.CommittedFiles);
  }
}
