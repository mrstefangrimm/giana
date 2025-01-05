using Giana.Api.Shared;
using System.Collections.Generic;
using System.IO;

namespace Giana.Api.Analysis.Coupling;

public record FileCoupling(string Name1, string Name2, int CouplingCount);

public static class FileCouplingActions
{
  public static void WriteAsCsv(this IEnumerable<FileCoupling> records, TextWriter writer)
  {
    foreach (var item in records)
    {
      writer.WriteLine($"{item.Name1}\t{item.Name2}\t{item.CouplingCount}");
    }
  }

  public static void Execute(ExecutionContext context)
  {
    var activitiesChart = FileCouplingCalculations.CreateFileCouplingList(context.LogRecords, context.ActiveNames);
    WriteAsCsv(activitiesChart, context.Output);
  }
}
