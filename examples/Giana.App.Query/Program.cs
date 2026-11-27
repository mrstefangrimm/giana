using Giana.App.Shared;
using System;

string gitExePath =
  OperatingSystem.IsWindows()
    ? @"C:\Program Files\Git\bin\git.exe"
    : "/usr/bin/git";
const string gitRepository = "https://github.com/dotnet/wpf.git";

var query = new Query
{
  Sources = [gitRepository],
  Analyzer = "author-ranking",
  OutputFormat = "csv",
  Renames = [new Author() { To = "Thomas Goulet", From = "ThomasGoulet73" }],
  TimeRanges = [new TimePeriod() { Begin = DateTime.Now.AddMonths(-6), End = DateTime.Now }]
};

var routine = query.CreateRoutine();

await routine.ExecuteAsync(gitExePath, Console.Out, TimeSpan.FromSeconds(60));
