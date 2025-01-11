using System.Collections.Immutable;
using System.IO;

namespace Giana.Api.Analysis.Coupling;

public record ProjectCouplingAndCohesion(string ProjectName, int CouplingCount, int CohesionCount, double Ratio);

public static class ProjectCouplingAndCohesionActions
{
  public static void WriteAsCsv(this IImmutableList<ProjectCouplingAndCohesion> records, TextWriter writer)
  {
    foreach (var item in records)
    {
      writer.WriteLine($"{item.ProjectName},{item.CouplingCount},{item.CohesionCount},{item.Ratio:0.000}");
    }
  }

  public static void Execute(ExecutionContext context)
  {
    var list = ProjectCouplingAndCohesionRankingCalculations.CreateProjectCouplingList(context.LogRecords, context.ActiveNames);
    WriteAsCsv(list, context.Output);
  }
}
