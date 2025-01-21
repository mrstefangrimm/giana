using Giana.Api.Analysis.Ranking;

namespace Giana.Api.Analysis.Tests;

public class AuthorRankingCalculationsTest : CalculationsTestBase
{
  [Fact]
  public void CreateAuthorRankingSorted_WithTestRecords_SortedByFileTouchesDescendng()
  {
    var result = _testRecords.CreateAuthorRankingSorted();
    Assert.Equal(7, result[0].FileTouches);
    Assert.Equal(2, result[1].FileTouches);
  }
}

