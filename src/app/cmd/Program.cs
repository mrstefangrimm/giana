using Giana.Api.Load;
using Giana.App.Shared;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using static Giana.App.Shared.Actions;

const string GitExePathEnvName = "GitExePath";

var cmdLineArgs = Environment.GetCommandLineArgs().ToList();

int idxHelp = Math.Max(cmdLineArgs.IndexOf("-h"), cmdLineArgs.IndexOf("--help"));
if (idxHelp > 0)
{
  Console.WriteLine("usage: Giana.Cmd.App.[bat|sh] [(-q | --query-file) <filename>] [(-o | output-file) <filename>]");
  Console.WriteLine();
  Console.WriteLine("--query-file\tjson file with the query arguments. By default, query.json for the bin folder is used.");
  Console.WriteLine("--output-file\tcsv filename for the results. By default, the results are written to the standard output.");

  return;
}

var stopwatch = new Stopwatch();
stopwatch.Restart();

string gitExePath = Environment.GetEnvironmentVariable(GitExePathEnvName);
if (string.IsNullOrEmpty(gitExePath))
{
  Console.WriteLine($"environment variable '{GitExePathEnvName}' not found.");
  return;
}

if (!File.Exists(gitExePath))
{
  Console.WriteLine($"File '{gitExePath}' defined with environment variable '{GitExePathEnvName}' not found.");
  return;
}

string queryFilename = null;
TextWriter outputWriter = null;

using var defer = new Defer(() =>
{
  if (outputWriter != null)
  {
    outputWriter.Flush();
    outputWriter.Dispose();
  }
});

int idxQueryFile = Math.Max(cmdLineArgs.IndexOf("-q"), cmdLineArgs.IndexOf("--query-file"));
if (idxQueryFile > 0 && cmdLineArgs.Count > idxQueryFile + 1)
{
  queryFilename = cmdLineArgs[idxQueryFile + 1];
}
else
{
  var cwd = Directory.GetCurrentDirectory();
  queryFilename = Path.Combine(cwd, "query.json");
}

if (!File.Exists(queryFilename))
{
  Console.WriteLine($"File '{queryFilename}' defined with the command line argument '{cmdLineArgs[idxQueryFile]}' not found.");
  return;
}

int idxOutputFile = Math.Max(cmdLineArgs.IndexOf("-o"), cmdLineArgs.IndexOf("--output-file"));
if (idxOutputFile > 0 && cmdLineArgs.Count > idxOutputFile + 1)
{
  try
  {
    var outputStream = File.Open(cmdLineArgs[idxOutputFile + 1], FileMode.Create);
    outputWriter = new StreamWriter(outputStream);
  }
  catch
  {
    Console.WriteLine($"Failed to create output file {cmdLineArgs[idxOutputFile + 1]} defined with the command line argument '{cmdLineArgs[idxOutputFile]}'.");
    return;
  }
}
else
{
  outputWriter = Console.Out;
}

if (!File.Exists(queryFilename))
{
  throw new FileNotFoundException(queryFilename);
}

using StreamReader jsonReader = new StreamReader(queryFilename);
var query = JsonConvert.DeserializeObject<Query>(jsonReader.ReadToEnd());
jsonReader.Close();

var routine = Calculations.CreateRoutine(query);

routine.OutputWriter = outputWriter;

await ExecuteAsync(routine, () => gitExePath, CancellationToken.None);

Console.WriteLine($"Time spent: {stopwatch.Elapsed.Seconds} sec");
