using Giana.Api.Analysis.Coupling;
using System.Linq;

namespace Giana.Api.Analysis.Tests;

public class FolderCouplingCalculationsTest : CalculationsTestBase
{
  [Fact]
  public void CreateFolderCouplingList_WithTestRecords_CouplingCountIsCorrect()
  {
    var result = _testRecords.CreateFolderCouplingList(_activeNames);
    Assert.Equal(5, result.Count()); // 5 folders

    var root = result.First(x => x.FolderName == string.Empty);

    Assert.Equal(27, root.CohesionCount); // root folder has 100% cohesion, 9 commits, File0A.cs and Folder1/File1A.cs have a coupling of 2.
    Assert.Equal(0, root.CouplingCount); // root folder has 100% cohesion

    var properties = result.First(x => x.FolderName == "Properties/");
    Assert.Equal(0, properties.CohesionCount); // Properties has only has one file, the cohesion is 0.
    Assert.Equal(1, properties.CouplingCount); // launchSettings.json was changed once.

    var folder1 = result.First(x => x.FolderName == "Folder1/");
    Assert.Equal(2, folder1.CohesionCount); // Folder1/Folder1.csproj and Folder1/File1A.cs were committed together; the cohesion is 2.
    Assert.Equal(15, folder1.CouplingCount);

    var folder2 = result.First(x => x.FolderName == "Folder2/");
    Assert.Equal(6, folder2.CohesionCount);
    Assert.Equal(15, folder2.CouplingCount);

    var folder21 = result.First(x => x.FolderName == "Folder2/Folder21/");
    Assert.Equal(2, folder21.CohesionCount); // Folder2/Folder21/File21A.cs and Folder2/Folder21/File21B.cs were changed together; the cohesion is 2.
    Assert.Equal(14, folder21.CouplingCount);
  }
}
