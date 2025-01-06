using Giana.Api.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Giana.Api.Analysis.Coupling;

public static class FileCouplingCalculations
{
  public static ImmutableList<FileCoupling> CreateFileCouplingList(this IEnumerable<GitLogRecord> logRecords, IEnumerable<string> activeNames)
  {
    var couplingCounts = new Dictionary<string, Dictionary<string, int>>();

    // initialized twice as big as needed
    foreach (var name in activeNames)
    {
      couplingCounts[name] = new Dictionary<string, int>();
      foreach (var name2 in activeNames)
      {
        couplingCounts[name][name2] = 0;
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

    List<FileCoupling> fileCouplings = new List<FileCoupling>();
    foreach (var counts in couplingCounts)
    {
      foreach (var coupling in counts.Value.Where(x => x.Value > 0))
      {
        fileCouplings.Add(new FileCoupling(Name1: counts.Key, Name2: coupling.Key, CouplingCount: coupling.Value));
      }
    }

    return fileCouplings.ToImmutableList();
  }
}
