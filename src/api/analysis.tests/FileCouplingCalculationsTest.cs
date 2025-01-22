using Giana.Api.Analysis.Coupling;
using System.Collections.Immutable;
using System.Linq;

namespace Giana.Api.Analysis.Tests;

public class FileCouplingCalculationsTest : CalculationsTestBase
{
  [Fact]
  public void CreateFileCouplingList_WithTestRecords_CouplingCountIsCorrect()
  {
    var result = _testRecords.CreateFileCouplingList(_activeNames);
    Assert.Null(result.FirstOrDefault(x => x.Name1 == "File0B.cs" || x.Name2 == "File0B.cs")); // File0B.cs is not in _activeNames.
    Assert.Null(result.FirstOrDefault(x => x.Name1 == x.Name2));
    TestCouplingCount(result, "File0A.cs", "Folder1/File1A.cs", 2);
    TestCouplingCount(result, "File0A.cs", "Folder2/Folder21/File21A.cs", 1);
  }

  private static void TestCouplingCount(IImmutableList<FileCoupling> result, string name1, string name2, int count)
  {
    var coupling = result.FirstOrDefault(x => x.Name1 == name1 && x.Name2 == name2);
    if (coupling == null)
    {
      coupling = result.FirstOrDefault(x => x.Name1 == name2 && x.Name2 == name1);
    }
    Assert.Equal(count, coupling.CouplingCount);
  }
}
