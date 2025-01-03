using Giana.Api.Shared;
using System.Collections.Generic;
using System.IO;

namespace Giana.Api.Analysis.Activity;

public record AuthorActivity(string Author, string YearAndWeek, int TouchedFiles);

public static class AuthorActivityActions
{
  public static void WriteChartAsCsv(this IEnumerable<string> chart, TextWriter writer)
  {
    foreach (var row in chart)
    {
      writer.WriteLine(row);
    }
  }

  public static void Execute(IEnumerable<GitLogRecord> records, IEnumerable<string> activeNames, StreamWriter writer)
  {
    var activitiesChart = AuthorActivityCalculations.CreateActivityChartAsCsv(records);
    WriteChartAsCsv(activitiesChart, writer);
  }
}
