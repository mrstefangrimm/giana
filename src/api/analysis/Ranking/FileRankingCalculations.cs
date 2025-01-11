using Giana.Api.Core;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Giana.Api.Analysis.Ranking;

public static class FileRankingCalculations
{
  public static IImmutableList<FileRanking> CreateFileRankingSorted(this IImmutableList<GitLogRecord> records)
  {
    var groupedByPath = records.GroupBy(x => x.Name);
    var nameRanking = groupedByPath.Select(group =>
    {
      var record = group.First();
      return new FileRanking(record.RepoName, record.Name, group.Count());
    });

    return nameRanking.ToImmutableList().Sort((a, b) => b.ChangeCount - a.ChangeCount);
  }
}
