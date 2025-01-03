using Giana.Api.Shared;
using System.Collections.Generic;
using System.IO;

namespace Giana.Api.Analysis.Ranking;

public record FileRanking(string Path, int ChangeCount);

public static class FileRankingActions
{
  public static void WriteToCsv(this ICollection<FileRanking> records, StreamWriter writer)
  {
    foreach (var item in records)
    {
      writer.WriteLine($"{item.Path};{item.ChangeCount}");
    }
  }

  public static void Execute(IEnumerable<GitLogRecord> records, IEnumerable<string> activeNames, StreamWriter writer)
  {
    var ranking = FileRankingCalculations.CreateFileRankingSorted(records);
    WriteToCsv(ranking, writer);
  }
}
