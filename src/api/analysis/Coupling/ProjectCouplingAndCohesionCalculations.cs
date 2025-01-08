using Giana.Api.Core;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Giana.Api.Analysis.Coupling;

public static class ProjectCouplingAndCohesionRankingCalculations
{
  public static ImmutableList<ProjectCouplingAndCohesion> CreateProjectCouplingList(this IEnumerable<GitLogRecord> logRecords, IEnumerable<string> activeNames)
  {
    // logRecords can include historical items which are no longer active.
    var reducedNamesFromRecords = logRecords.Select(x => x.Name).Distinct().ToList();
    var usedNames = activeNames.Where(x => reducedNamesFromRecords.Contains(x)).ToList();

    var fileCouplings = FileCouplingCalculations.CreateFileCouplingList(logRecords, usedNames);

    var projectFileNames = usedNames.Where(x => x.EndsWith(".csproj"));

    var projectCouplingRecords = new List<ProjectCouplingAndCohesion>();
    foreach (var projectFileName in projectFileNames)
    {
      string projectPath = Calculations.GetPath(projectFileName);

      var recordsOfThisProject = fileCouplings.Where(rec => rec.Name1.StartsWith(projectPath) || rec.Name2.StartsWith(projectPath)).ToList();

      var intraProjectCommits = recordsOfThisProject.Where(rec => rec.Name1.StartsWith(projectPath) && rec.Name2.StartsWith(projectPath)).ToList();
      var interProjectCommits = recordsOfThisProject.Except(intraProjectCommits).ToList();

      int couplingCount = interProjectCommits.Sum(rec => rec.CouplingCount);
      int cohesionCount = intraProjectCommits.Sum(rec => rec.CouplingCount);

      double ratio = couplingCount > 0 ? cohesionCount * 1.0 / couplingCount : double.NaN;

      projectCouplingRecords.Add(new ProjectCouplingAndCohesion(projectFileName, couplingCount, cohesionCount, ratio));
    }

    return projectCouplingRecords.ToImmutableList();
  }
}