using Giana.Api.Shared;
using System.Collections.Generic;
using System.IO;

namespace Giana.Api.Analysis.Ranking;

public record CommitRanking(string Commit, string Description, int ChangedFiles);

public static class CommitRankingActions
{
  public static void WriteToCsv(this ICollection<CommitRanking> records, StreamWriter writer)
  {
    foreach (var item in records)
    {
      writer.WriteLine($"{item.Commit};{item.ChangedFiles};{item.Description}");
    }
  }

  public static void Execute(IEnumerable<GitLogRecord> records, IEnumerable<string> activeNames, StreamWriter writer)
  {
    var ranking = CommitRankingCalculations.CreateCommitRankingSorted(records);
    WriteToCsv(ranking, writer);
  }
}
