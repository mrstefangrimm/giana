using System.Collections.Immutable;
using System.IO;

namespace Giana.Api.Analysis.Activity;

public record AuthorActivity(string Author, string YearAndWeek, int TouchedFiles);

[Analyzer("author-activity")]
public static class AuthorActivityActions
{
  public static void WriteChartAsCsv(this IImmutableList<string> chart, TextWriter writer)
  {
    foreach (var row in chart)
    {
      writer.WriteLine(row);
    }
  }

  [AnalyzerExecute(["csv"])]
  public static void Execute(ExecutionContext context)
  {
    var activitiesChart = AuthorActivityCalculations.CreateActivityChartAsCsv(context.LogRecords);
    WriteChartAsCsv(activitiesChart, context.Output);
  }
}
