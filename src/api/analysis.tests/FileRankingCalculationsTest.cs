using Giana.Api.Analysis.Ranking;
using System.Linq;

namespace Giana.Api.Analysis.Tests;

public class FileRankingCalculationsTest : CalculationsTestBase
{
  [Fact]
  public void CreateFileRankingSorted_WithTestRecords_SortedByChangeCountDescendng()
  {
    var result = _testRecords.CreateFileRankingSorted();
    Assert.Equal("Readme.md", result.First().Name);
    Assert.Equal(3, result.First().ChangeCount);
    Assert.Equal("Folder1/File1A.cs", result.Second().Name);
    Assert.Equal(2, result.Second().ChangeCount);
    Assert.Equal(1, result.Last().ChangeCount);
  }
}
