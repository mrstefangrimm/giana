using Giana.Api.Shared;
using System.Collections.Generic;
using System.IO;

namespace Giana.Api.Analysis.Coupling;

public record FolderCouplingAndCohesion(string ProjectName, int CouplingCount, int CohesionCount, double Ratio);

public static class FolderCouplingAndCohesionActions
{
  public static void WriteAsCsv(this ICollection<FolderCouplingAndCohesion> records, TextWriter writer)
  {
    foreach (var item in records)
    {
      writer.WriteLine($"{item.ProjectName}\t{item.CouplingCount}\t{item.CohesionCount}\t{item.Ratio:0.000}");
    }
  }

  public static void Execute(ExecutionContext context)
  {
    var list = FolderCouplingAndCohesionRankingCalculations.CreateFolderCouplingList(context.LogRecords, context.ActiveNames);
    WriteAsCsv(list, context.Output);
  }
}