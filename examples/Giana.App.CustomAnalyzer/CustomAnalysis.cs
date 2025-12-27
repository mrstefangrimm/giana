using Giana.Api.Analysis;
using System;
using System.Collections.Generic;
using System.IO;

namespace Giana.App.Shared.Tests;

public record TestAnalysis(string Author, string Commit, string File);

[Analyzer("custom-analysis")]
public static class TestAnalyzerActions
{
  [AnalyzerExecute(["csv", "html"])]
  public static void Execute(ExecutionContext context)
  {
    var writers = new Dictionary<string, Action<List<TestAnalysis>, TextWriter>>
    {
      { "csv", WriteAsCsv },
      { "html", WriteAsHtml }
    };

    var last10 = new List<TestAnalysis>();
    for (int n = 0; n < Math.Min(10, context.LogRecords.Count); n++)
    {
      var record = context.LogRecords[n];
      last10.Add(new TestAnalysis(record.Author, record.Commit, record.Name));
    }

    writers[context.OutputFormat.ToLower()](last10, context.Output);
  }

  private static void WriteAsCsv(List<TestAnalysis> data, TextWriter writer)
  {
    writer.WriteLine("Author,Commit,File");
    foreach (var record in data)
    {
      writer.WriteLine($"{record.Author},{record.Commit},{record.File}");
    }
  }

  private static void WriteAsHtml(List<TestAnalysis> data, TextWriter writer)
  {
    writer.WriteLine("<html>");
    writer.WriteLine("  <body>");
    writer.WriteLine("    <h1>Last 10 changes</h1>");
    writer.WriteLine("    <table>");
    writer.WriteLine("      <tr><th>Author</th><th>Commit</th><th>File</th></tr>");
    foreach (var record in data)
    {
      writer.WriteLine($"      <tr><td>{record.Author}</td><td>{record.Commit}</td><td>{record.File}</td></tr>");
    }
    writer.WriteLine("    </table>");
    writer.WriteLine("  </body>");
    writer.WriteLine("</html>");
  }
}
