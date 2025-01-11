using Giana.Api.Core;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Giana.Api.Analysis.Coupling;

public static class FolderCouplingAndCohesionRankingCalculations
{
  public static IImmutableList<FolderCouplingAndCohesion> CreateFolderCouplingList(this IImmutableList<GitLogRecord> logRecords, IImmutableList<string> activeNames)
  {
    // logRecords can include historical items which are no longer active.
    var reducedNamesFromRecords = logRecords.Select(x => x.Name).Distinct().ToImmutableList();
    var usedNames = activeNames.Where(x => reducedNamesFromRecords.Contains(x)).ToImmutableList();

    var elementsWithLeaves = new List<string>();
    FillElementsList(0, "", usedNames, elementsWithLeaves);

    var fileCouplings = FileCouplingCalculations.CreateFileCouplingList(logRecords, usedNames);

    var projectCouplingRecords = new List<FolderCouplingAndCohesion>();
    foreach (var path in elementsWithLeaves)
    {
      var recordsOfThisProject = fileCouplings.Where(rec => rec.Name1.StartsWith(path) || rec.Name2.StartsWith(path)).ToList();

      var intraProjectCommits = recordsOfThisProject.Where(rec => rec.Name1.StartsWith(path) && rec.Name2.StartsWith(path)).ToList();
      var interProjectCommits = recordsOfThisProject.Except(intraProjectCommits).ToList();

      int couplingCount = interProjectCommits.Sum(rec => rec.CouplingCount);
      int cohesionCount = intraProjectCommits.Sum(rec => rec.CouplingCount);

      double ratio = couplingCount > 0 ? cohesionCount * 1.0 / couplingCount : double.NaN;

      projectCouplingRecords.Add(new FolderCouplingAndCohesion(path, couplingCount, cohesionCount, ratio));
    }

    return projectCouplingRecords.ToImmutableList();
  }

  private static void FillElementsList(int depth, string root, IImmutableList<string> activeNames, List<string> elementsWithLeaves)
  {
    var distinctElements = activeNames.Where(x => x.Split('/').Count() > depth).Select(x => x.Split('/')[depth]).Distinct().ToList();
    if (distinctElements.Count == 1)
    {
      FillElementsList(depth + 1, $"{root}{distinctElements[0]}/", activeNames, elementsWithLeaves);
    }
    else
    {
      foreach (var element in distinctElements)
      {
        var name = $"{root}{element}";
        int cnt = activeNames.Where(x => x.StartsWith(name)).ToList().Count;
        if (cnt == 1)
        {
          if (!elementsWithLeaves.Contains(root))
          {
            elementsWithLeaves.Add(root);
          }
        }
        else if (cnt > 1)
        {
          FillElementsList(depth + 1, $"{name}/", activeNames, elementsWithLeaves);
        }
      }
    }
  }
}