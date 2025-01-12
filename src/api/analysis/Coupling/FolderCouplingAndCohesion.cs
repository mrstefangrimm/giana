using System.Collections.Immutable;
using System.IO;

namespace Giana.Api.Analysis.Coupling;

public record FolderCouplingAndCohesion(string FolderName, int CouplingCount, int CohesionCount, double Ratio);

public static class FolderCouplingAndCohesionActions
{
  public static void WriteAsCsv(this IImmutableList<FolderCouplingAndCohesion> records, TextWriter writer)
  {
    writer.WriteLine("FolderName,CohesionCount,CouplingCount,Ratio Cohesion/Coupling");

    foreach (var item in records)
    {
      writer.WriteLine($"{item.FolderName},{item.CohesionCount},{item.CouplingCount},{item.Ratio:0.000}");
    }
  }

  public static void Execute(ExecutionContext context)
  {
    var list = FolderCouplingAndCohesionRankingCalculations.CreateFolderCouplingList(context.LogRecords, context.ActiveNames);
    WriteAsCsv(list, context.Output);
  }
}