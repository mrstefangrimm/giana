using Giana.Api.Analysis;
using System;
using System.Collections.Generic;
using System.IO;

namespace Giana.App.Shared.Tests;

public record TestAnalysis(string Property1, string Property2);

[Analyzer("test-analysis")]
public static class TestAnalyzerActions
{
  [AnalyzerExecute(["csv", "json", "html"])]
  public static void Execute(ExecutionContext context)
  {
    var writers = new Dictionary<string, Action<TestAnalysis, TextWriter>>
    {
      { "csv", WriteAsCsv },
      { "json", WriteAsJson },
      { "html", WriteAsHtml }
    };

    var data = new TestAnalysis("p1", "p2");
    writers[context.OutputFormat.ToLower()](data, context.Output);
  }

  private static void WriteAsCsv(TestAnalysis data, TextWriter writer)
  {
    writer.WriteLine("Property1,Property2");
    writer.WriteLine($"{data.Property1},{data.Property2}");
  }

  private static void WriteAsJson(TestAnalysis data, TextWriter writer)
  {
    writer.WriteLine($"{{ Property1: {data.Property1}, Property2: {data.Property2} }}");
  }

  private static void WriteAsHtml(TestAnalysis data, TextWriter writer)
  {
    writer.WriteLine($"<html><Property1>{data.Property1}</Property1>,<Property2>{data.Property2}</Property2>");
  }
}
