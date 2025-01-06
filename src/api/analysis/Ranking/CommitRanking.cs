using Giana.Api.Shared;
using System.Collections.Generic;
using System.IO;

namespace Giana.Api.Analysis.Ranking;

public record CommitRanking(string Commit, string Description, int ChangedFiles);

public static class CommitRankingActions
{
  public static void WriteAsCsv(this ICollection<CommitRanking> records, StreamWriter writer)
  {
    foreach (var item in records)
    {
      writer.WriteLine($"{item.Commit}\t{item.ChangedFiles}\t{item.Description}");
    }
  }

  public static void Execute(ExecutionContext context)
  {
    var ranking = CommitRankingCalculations.CreateCommitRankingSorted(context.LogRecords);
    WriteAsCsv(ranking, context.Output);
  }
}
