using Giana.Api.Shared;
using System.Collections.Generic;
using System.IO;

namespace Giana.Api.Analysis.Coupling;

public record ProjectCouplingAndCohesion(string ProjectName, int CouplingCount, int CohesionCount, double Ratio);

public static class ProjectCouplingAndCohesionActions
{
  public static void WriteAsCsv(this IEnumerable<ProjectCouplingAndCohesion> records, TextWriter writer)
  {
    foreach (var item in records)
    {
      writer.WriteLine($"{item.ProjectName}\t{item.CouplingCount}\t{item.CohesionCount}\t{item.Ratio:0.000}");
    }
  }

  public static void Execute(ExecutionContext context)
  {
    var list = ProjectCouplingAndCohesionRankingCalculations.CreateProjectCouplingList(context.LogRecords, context.ActiveNames);
    WriteAsCsv(list, context.Output);
  }
}
