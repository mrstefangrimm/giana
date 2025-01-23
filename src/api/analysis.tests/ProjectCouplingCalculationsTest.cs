using Giana.Api.Analysis.Coupling;
using System.Linq;

namespace Giana.Api.Analysis.Tests;

public class ProjectCouplingCalculationsTest : CalculationsTestBase
{
  [Fact]
  public void CreateProjectCouplingList_WithTestRecords_CouplingCountIsCorrect()
  {
    var result = _testRecords.CreateProjectCouplingList(_activeNames);
    Assert.Equal(2, result.Count()); // 2 projects

    var folder1Csproj = result.First(x => x.ProjectName == "Folder1/Folder1.csproj");
    Assert.Equal(20, folder1Csproj.CohesionCount);
    Assert.Equal(6, folder1Csproj.CouplingCount);

    var folder2Csproj = result.First(x => x.ProjectName == "Folder2/Folder2.csproj");
    Assert.Equal(20, folder2Csproj.CohesionCount);
    Assert.Equal(6, folder2Csproj.CouplingCount);
  }
}
