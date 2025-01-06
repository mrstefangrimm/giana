using Giana.Api.Core;
using System.Collections.Generic;
using System.IO;

namespace Giana.Api.Analysis.Ranking;

public record FileRanking(string Path, int ChangeCount);

public static class FileRankingActions
{
  public static void WriteAsCsv(this ICollection<FileRanking> records, TextWriter writer)
  {
    foreach (var item in records)
    {
      writer.WriteLine($"{item.Path}\t{item.ChangeCount}");
    }
  }

  public static void Execute(ExecutionContext context)
  {
    var ranking = FileRankingCalculations.CreateFileRankingSorted(context.LogRecords);
    WriteAsCsv(ranking, context.Output);
  }
}
