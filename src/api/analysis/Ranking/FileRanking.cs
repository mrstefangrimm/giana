using System.Collections.Immutable;
using System.IO;

namespace Giana.Api.Analysis.Ranking;

public record FileRanking(string Repository, string Name, int ChangeCount);

public static class FileRankingActions
{
  public static void WriteAsCsv(this IImmutableList<FileRanking> records, TextWriter writer)
  {
    writer.WriteLine("Repository,FileName,ChangeCount");

    foreach (var item in records)
    {
      writer.WriteLine($"{item.Repository},{item.Name},{item.ChangeCount}");
    }
  }

  public static void Execute(ExecutionContext context)
  {
    var ranking = FileRankingCalculations.CreateFileRankingSorted(context.LogRecords);
    WriteAsCsv(ranking, context.Output);
  }
}
