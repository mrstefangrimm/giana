using Giana.Api.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Giana.Api.Analysis.Coupling;

public static class FileCouplingCalculations
{
  public static IImmutableList<FileCoupling> CreateFileCouplingList(this IImmutableList<GitLogRecord> logRecords, IImmutableList<string> activeNames)
  {
    // logRecords can include historical items which are no longer active.
    var reducedNamesFromRecords = logRecords.Select(x => x.Name).Distinct().ToList();
    var usedNames = activeNames.Where(x => reducedNamesFromRecords.Contains(x)).ToList();

    var couplingCounts = new Dictionary<string, Dictionary<string, int>>();

    // The array is initialized twice as big as needed. But calculating the coupling this way is very fast.
    foreach (var nameCol in usedNames)
    {
      couplingCounts[nameCol] = new Dictionary<string, int>();
      foreach (var nameRow in usedNames)
      {
        couplingCounts[nameCol][nameRow] = 0;
      }
    }

    var changesByCommit = logRecords.GroupBy(change => change.Commit);
    foreach (var change in changesByCommit)
    {
      var committedFiles = change.ToArray();

      for (int n = 0; n < committedFiles.Count(); n++)
      {
        for (int m = 1; m < committedFiles.Count(); m++)
        {
          if (couplingCounts.ContainsKey(committedFiles[n].Name) && couplingCounts.ContainsKey(committedFiles[m].Name))
          {
            couplingCounts[committedFiles[n].Name][committedFiles[m].Name] += 1;
          }
        }
      }
    }

    var fileCouplings = new List<FileCoupling>();
    foreach (var counts in couplingCounts)
    {
      foreach (var coupling in counts.Value.Where(x => x.Value > 0))
      {
        // Remove the cells with the same name in both axes
        if (counts.Key == coupling.Key) continue;

        fileCouplings.Add(new FileCoupling(Name1: counts.Key, Name2: coupling.Key, CouplingCount: coupling.Value));
      }
    }

    return fileCouplings.ToImmutableList();
  }
}
