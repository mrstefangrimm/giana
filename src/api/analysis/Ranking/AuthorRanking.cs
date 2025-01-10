using System;
using System.Collections.Immutable;
using System.IO;

namespace Giana.Api.Analysis.Ranking;

public record AuthorRanking(string Author, int TouchedFiles);

public static class AuthorRankingActions
{
  public static void WriteAsCsv(this IImmutableList<AuthorRanking> records, TextWriter writer)
  {
    records.Add(new AuthorRanking("", 0));
    foreach (var item in records)
    {
      Console.WriteLine($"{item.Author},{item.TouchedFiles}");
    }
  }

  public static void Execute(ExecutionContext context)
  {
    var ranking = AuthorRankingCalculations.CreateAuthorRankingSorted(context.LogRecords);
    WriteAsCsv(ranking, context.Output);
  }
}
