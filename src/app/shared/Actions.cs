using Giana.Api.Analysis;
using Giana.Api.Core;
using Giana.Api.Load;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static Giana.App.Shared.Calculations;

namespace Giana.App.Shared;

public static class Actions
{
  private static readonly object _lock = new object();

  public static async Task ExecuteAsync(this Routine routine, string gitExePath, TextWriter outputWriter, TimeSpan timeout)
  {
    using var cancellationSource = new System.Threading.CancellationTokenSource(timeout);

    ImmutableList<GitLogRecord> reducedRecords = [];
    ImmutableList<string> allActiveNames = [];

    try
    {
      await Parallel.ForEachAsync(routine.Sources, cancellationSource.Token, async (source, cancellationToken) =>
      {
        using var gitRepo = await GitRepository.CreateAsync(source, gitExePath, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        var records = await gitRepo.LogAsync(cancellationToken, routine.CommitsFrom);

        records = records.Where(x => routine.TimeRanges.Count == 0 || routine.TimeRanges.Any(tp => tp.Begin <= x.Date && x.Date <= tp.End)).ToImmutableList();

        foreach (var renameAuthor in routine.Renames)
        {
          records = renameAuthor.Invoke(records, renameAuthor.To, renameAuthor.From);
        }

        foreach (var reduction in routine.Reductions)
        {
          records = reduction.Invoke(records, reduction.Argument);
        }

        lock (_lock)
        {
          reducedRecords = reducedRecords.AddRange(records);
        }

        // reducedNamesFromRecords can include historical items which are no longer active.
        var reducedNamesFromRecords = reducedRecords.Select(x => x.Name).Distinct();
        var reducedActiveNames = (await gitRepo.ActiveNamesAsync(cancellationToken)).Where(x => reducedNamesFromRecords.Contains(x));
        lock (_lock)
        {
          allActiveNames = allActiveNames.AddRange(reducedActiveNames);
        }
      });

      routine.Analyze(new Api.Analysis.ExecutionContext(reducedRecords, allActiveNames, routine.OutputFormat, outputWriter, cancellationSource.Token));
    }
    catch (OperationCanceledException)
    {
      Console.WriteLine($"`{Name()}` timeout of {(int)timeout.TotalMilliseconds} ms reached. Ended without results.");
    }
  }

  public static IImmutableDictionary<string, (string[], Action<ExecutionContext>)> WithCustomAnalyzers(this IImmutableDictionary<string, (string[], Action<ExecutionContext>)> analyzers, string assemblyString)
  {
    var assembly = Assembly.Load(assemblyString);

    var analzerExecutors = new Dictionary<string, (string[], Action<ExecutionContext>)>();
    var customAnalyzerTypes = assembly.GetTypes().Where(t => t.GetCustomAttribute(typeof(AnalyzerAttribute)) != null);

    foreach (var analyzer in customAnalyzerTypes)
    {
      var executeMethod = analyzer.GetMethods(BindingFlags.Public | BindingFlags.Static).First(m => m.GetCustomAttribute(typeof(AnalyzerExecuteAttribute)) != null);
      var outputFormats = executeMethod.GetCustomAttribute<AnalyzerExecuteAttribute>().AnalyzerExecute;

      var reflectionExecutor = new Action<ExecutionContext>(context =>
      {
        executeMethod.Invoke(analyzer, [context]);
      });
      analzerExecutors.Add(analyzer.GetCustomAttribute<AnalyzerAttribute>().Analyzer, (outputFormats, reflectionExecutor));
    }

    return analyzers.AddRange(analzerExecutors.ToImmutableDictionary());
  }
}
