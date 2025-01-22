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

    var folderCouplingRecords = new List<FolderCouplingAndCohesion>();
    foreach (var path in elementsWithLeaves)
    {
      var recordsOfThisFolder = fileCouplings.Where(rec => rec.Name1.StartsWith(path) || rec.Name2.StartsWith(path)).ToList();

      var intraFolderCommits = recordsOfThisFolder.Where(rec => rec.Name1.StartsWith(path) && rec.Name2.StartsWith(path)).ToList();
      var interFolderCommits = recordsOfThisFolder.Except(intraFolderCommits).ToList();

      int couplingCount = interFolderCommits.Sum(rec => rec.CouplingCount);
      int cohesionCount = intraFolderCommits.Sum(rec => rec.CouplingCount);

      double ratio = couplingCount > 0 ? cohesionCount * 1.0 / couplingCount : double.NaN;

      folderCouplingRecords.Add(new FolderCouplingAndCohesion(path, couplingCount, cohesionCount, ratio));
    }

    return folderCouplingRecords.ToImmutableList();
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
        int cnt = activeNames.Count(x => x.StartsWith(name));
        if (cnt == 1)
        {
          // leaf
          bool isLeaf = activeNames.Any(x => x.EndsWith(name));
          if (isLeaf)
          {
            if (!elementsWithLeaves.Contains(root))
            {
              elementsWithLeaves.Add(root);
            }
          }
          else
          {
            // folder
            FillElementsList(depth + 1, $"{name}/", activeNames, elementsWithLeaves);
          }
        }
        else
        {
          // folder
          FillElementsList(depth + 1, $"{name}/", activeNames, elementsWithLeaves);
        }
      }
    }
  }
}