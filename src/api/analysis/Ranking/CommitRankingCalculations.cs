using Giana.Api.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Giana.Api.Analysis.Ranking;

public static class CommitRankingCalculations
{
  public static ImmutableList<CommitRanking> CreateCommitRankingSorted(this IEnumerable<GitLogRecord> records)
  {
    var groupedByCommit = records.GroupBy(x => x.Commit);
    var commitRanking = groupedByCommit.Select(group =>
    {
      var commit = group.First();
      return new CommitRanking(commit.Commit, commit.Message, group.Count());
    });

    return commitRanking.ToImmutableList().Sort((a, b) => b.ChangedFiles - a.ChangedFiles);
  }
}
