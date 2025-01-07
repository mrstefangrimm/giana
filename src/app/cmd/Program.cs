using Giana.App.Shared;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using static Giana.App.Shared.Actions;

var cmdLineArgs = Environment.GetCommandLineArgs().ToList();

string queryFilename = null;
TextWriter outputWriter = null;

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

int idxOutputFile = Math.Max(cmdLineArgs.IndexOf("-o"), cmdLineArgs.IndexOf("--output-file"));
if (idxOutputFile > 0 && cmdLineArgs.Count > idxOutputFile + 1)
{
  var outputStream = File.Open(cmdLineArgs[idxOutputFile + 1], FileMode.OpenOrCreate);
  outputWriter = new StreamWriter(outputStream);
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

var gitLogRecords = Execute(routine);

outputWriter.Flush();
outputWriter.Dispose();
