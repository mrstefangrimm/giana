using Giana.App.Shared;
using System;

const string gitExePath = @"C:\Program Files\Git\bin\git.exe";
const string gitRepository = "https://github.com/dotnet/wpf.git";

var query = new Query
{
  Sources = [gitRepository],
  Analyzer = "author-ranking",
  OutputFormat = "csv",
  Renames = [new Author() { To = "Thomas Goulet", From = "ThomasGoulet73" }]
};

var routine = query.CreateRoutine(Console.Out);

await Actions.ExecuteAsync(routine, () => gitExePath, 60000);
