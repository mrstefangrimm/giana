using Giana.App.Shared;
using System;

const string gitExePath = @"C:\Git2\bin\git.exe";
const string gitRepository = "https://github.com/mrstefangrimm/giana.git";

var query = new Query
{
  Sources = [gitRepository],
  Analyzer = "author-ranking",
  OutputFormat = "csv",  
  Renames = [new Author() { To = "Stefan Grimm", From = "mrstefangrimm" }]
};

var routine = query.CreateRoutine();
routine.OutputWriter = Console.Out;

await Giana.App.Shared.Actions.ExecuteAsync(routine, () => gitExePath, 10000);
