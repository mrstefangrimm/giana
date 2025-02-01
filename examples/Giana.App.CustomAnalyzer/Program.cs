using Giana.App.Shared;
using System;

const string gitExePath = @"C:\Program Files\Git\bin\git.exe";
const string gitRepository = "https://github.com/dotnet/wpf.git";

var query = new Query
{
  Sources = [gitRepository],
  Analyzer = "custom-analysis",
  OutputFormat = "html",
  Renames = [new Author() { To = "Thomas Goulet", From = "ThomasGoulet73" }],
  TimeRanges = [new TimePeriod() { Begin = DateTime.Now.AddMonths(-6), End = DateTime.Now }]
};

var analyzers = Calculations.GetDefaultAnalyzers();
analyzers = analyzers.WithCustomAnalyzers("Giana.App.CustomAnalyzer");

var routine = query.CreateRoutine(analyzers);

await routine.ExecuteAsync(gitExePath, Console.Out, TimeSpan.FromSeconds(60));
