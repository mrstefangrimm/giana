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
      writer.WriteLine($"{item.ProjectName};{item.CouplingCount};{item.CohesionCount};{item.Ratio:0.000}");
    }
  }

  public static void Execute(IEnumerable<GitLogRecord> records, IEnumerable<string> activeNames, StreamWriter writer)
  {
    var list = ProjectCouplingAndCohesionRankingCalculations.CreateProjectCouplingList(records, activeNames);
    WriteAsCsv(list, writer);
  }
}
