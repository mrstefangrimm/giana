using Giana.Api.Shared;
using System;
using System.Collections.Generic;
using System.IO;

namespace Giana.Api.Analysis.Ranking;

public record AuthorRanking(string Author, int TouchedFiles);

public static class AuthorRankingActions
{
  public static void WriteToCsv(this ICollection<AuthorRanking> records, StreamWriter writer)
  {
    foreach (var item in records)
    {
      Console.WriteLine($"{item.Author};{item.TouchedFiles}");
    }
  }

  public static void Execute(IEnumerable<GitLogRecord> records, IEnumerable<string> activeNames, StreamWriter writer)
  {
    var ranking = AuthorRankingCalculations.CreateAuthorRankingSorted(records);
    WriteToCsv(ranking, writer);
  }
}
