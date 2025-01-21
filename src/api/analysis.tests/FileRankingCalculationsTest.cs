using Giana.Api.Analysis.Ranking;

namespace Giana.Api.Analysis.Tests;

public class FileRankingCalculationsTest : CalculationsTestBase
{
  [Fact]
  public void CreateFileRankingSorted_WithTestRecords_SortedByChangeCountDescendng()
  {
    var result = _testRecords.CreateFileRankingSorted();
    Assert.Equal(3, result[0].ChangeCount);
    Assert.Equal(2, result[1].ChangeCount);
    Assert.Equal(2, result[2].ChangeCount);
    Assert.Equal(1, result[3].ChangeCount);
    Assert.Equal(1, result[4].ChangeCount);
  }
}
