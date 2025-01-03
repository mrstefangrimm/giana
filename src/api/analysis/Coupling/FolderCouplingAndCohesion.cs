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
      writer.WriteLine($"{item.ProjectName};{item.CouplingCount};{item.CohesionCount};{item.Ratio:0.000}");
    }
  }

  public static void Execute(IEnumerable<GitLogRecord> records, IEnumerable<string> activeNames, StreamWriter writer)
  {
    var list = FolderCouplingAndCohesionRankingCalculations.CreateFolderCouplingList(records, activeNames);
    WriteAsCsv(list, writer);
  }
}