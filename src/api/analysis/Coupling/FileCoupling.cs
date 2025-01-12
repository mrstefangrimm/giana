using System.Collections.Immutable;
using System.IO;

namespace Giana.Api.Analysis.Coupling;

public record FileCoupling(string Name1, string Name2, int CouplingCount);

public static class FileCouplingActions
{
  public static void WriteAsCsv(this IImmutableList<FileCoupling> records, TextWriter writer)
  {
    writer.WriteLine("FileName1,FileName2,CouplingCount");

    foreach (var item in records)
    {
      writer.WriteLine($"{item.Name1},{item.Name2},{item.CouplingCount}");
    }
  }

  public static void Execute(ExecutionContext context)
  {
    var activitiesChart = FileCouplingCalculations.CreateFileCouplingList(context.LogRecords, context.ActiveNames);
    WriteAsCsv(activitiesChart, context.Output);
  }
}
