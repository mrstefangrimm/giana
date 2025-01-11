using System.Collections.Immutable;
using System.IO;

namespace Giana.Api.Analysis.Ranking;

public record CommitRanking(string Repository, string Commit, string Description, int ChangedFiles);

public static class CommitRankingActions
{
  public static void WriteAsCsv(this IImmutableList<CommitRanking> records, TextWriter writer)
  {
    foreach (var item in records)
    {
      writer.WriteLine($"{item.Repository},{item.Commit},{item.ChangedFiles},{item.Description}");
    }
  }

  public static void Execute(ExecutionContext context)
  {
    var ranking = CommitRankingCalculations.CreateCommitRankingSorted(context.LogRecords);
    WriteAsCsv(ranking, context.Output);
  }
}
