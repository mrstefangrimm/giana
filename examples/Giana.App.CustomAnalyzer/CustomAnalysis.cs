using Giana.Api.Analysis;
using Giana.Api.Analysis.Ranking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Giana.App.Shared.Tests;

public record TestAnalysis(string Author, string Commit);

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

    var groupedByCommit = context.LogRecords.GroupBy(x => x.Commit).Distinct().ToList();
    var commits = groupedByCommit.Select(group =>
    {
      var commit = group.First();
      return new TestAnalysis(commit.Author, commit.Commit);
    }).ToList();

    var last10 = new List<TestAnalysis>();
    for (int n = 0; n < Math.Min(10, commits.Count); n++)
    {

      last10.Add(commits[n]);
    }

    writers[context.OutputFormat.ToLower()](last10, context.Output);
  }

  private static void WriteAsCsv(List<TestAnalysis> data, TextWriter writer)
  {
    writer.WriteLine("Author,Commit,File");
    foreach (var record in data)
    {
      writer.WriteLine($"{record.Author},{record.Commit}");
    }
  }

  private static void WriteAsHtml(List<TestAnalysis> data, TextWriter writer)
  {
    writer.WriteLine("<html>");
    writer.WriteLine("  <body>");
    writer.WriteLine("    <h1>Last 10 changes</h1>");
    writer.WriteLine("    <table>");
    writer.WriteLine("      <tr><th>Author</th><th>Commit</th></tr>");
    foreach (var record in data)
    {
      writer.WriteLine($"      <tr><td>{record.Author}</td><td>{record.Commit}</td></tr>");
    }
    writer.WriteLine("    </table>");
    writer.WriteLine("  </body>");
    writer.WriteLine("</html>");
  }
}
