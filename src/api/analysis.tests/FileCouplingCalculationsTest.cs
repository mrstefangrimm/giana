using Giana.Api.Analysis.Coupling;
using System.Linq;

namespace Giana.Api.Analysis.Tests;

public class FileCouplingCalculationsTest : CalculationsTestBase
{
  [Fact]
  public void CreateFileCouplingList_WithTestRecords_SortedByCommittedFilesDescendng()
  {
    var result = _testRecords.CreateFileCouplingList(_activeNames);
    Assert.Null(result.FirstOrDefault(x => x.Name1 == "File0B.cs" || x.Name2 == "File0B.cs"));
  }
}

