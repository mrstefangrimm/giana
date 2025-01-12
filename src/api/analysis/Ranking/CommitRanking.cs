using System.Collections.Immutable;
using System.IO;

namespace Giana.Api.Analysis.Ranking;

public record CommitRanking(string Repository, string Commit, string Message, int CommittedFiles);

public static class CommitRankingActions
{
  public static void WriteAsCsv(this IImmutableList<CommitRanking> records, TextWriter writer)
  {
    writer.WriteLine("Repository,Commit,CommitedFiles,Commit Message");

    foreach (var item in records)
    {
      writer.WriteLine($"{item.Repository},{item.Commit},{item.CommittedFiles},{item.Message}");
    }
  }

  public static void Execute(ExecutionContext context)
  {
    var ranking = CommitRankingCalculations.CreateCommitRankingSorted(context.LogRecords);
    WriteAsCsv(ranking, context.Output);
  }
}
