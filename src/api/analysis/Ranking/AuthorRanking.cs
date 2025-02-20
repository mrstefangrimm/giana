﻿using System.Collections.Immutable;
using System.IO;

namespace Giana.Api.Analysis.Ranking;

public record AuthorRanking(string Author, int FileTouches);

[Analyzer("author-ranking")]
public static class AuthorRankingActions
{
  public static void WriteAsCsv(this IImmutableList<AuthorRanking> records, TextWriter writer)
  {
    writer.WriteLine("Author,FileTouches");

    foreach (var item in records)
    {
      writer.WriteLine($"{item.Author},{item.FileTouches}");
    }
  }

  [AnalyzerExecute(["csv"])]
  public static void Execute(ExecutionContext context)
  {
    var ranking = AuthorRankingCalculations.CreateAuthorRankingSorted(context.LogRecords);
    WriteAsCsv(ranking, context.Output);
  }
}
