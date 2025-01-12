using Giana.Api.Core;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Giana.Api.Analysis.Ranking;

public static class AuthorRankingCalculations
{
  public static IImmutableList<AuthorRanking> CreateAuthorRankingSorted(this IImmutableList<GitLogRecord> records)
  {
    //var authorRanking = from item in records.AsParallel()
    //                    group item by item.Author into grp
    //                    select new AuthorRanking(grp.First().Author, grp.Count());

    var groupedByAuthor = records.GroupBy(x => x.Author);
    var authorRanking = groupedByAuthor.Select(group =>
    {
      var record = group.First();
      return new AuthorRanking(record.Author, group.Count());
    });

    return authorRanking.ToImmutableList().Sort((a, b) => b.FileTouches - a.FileTouches);
  }
}


