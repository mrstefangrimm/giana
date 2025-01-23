using Giana.Api.Analysis.Ranking;
using System.Linq;

namespace Giana.Api.Analysis.Tests;

public class AuthorRankingCalculationsTest : CalculationsTestBase
{
  [Fact]
  public void CreateAuthorRankingSorted_WithTestRecords_SortedByFileTouchesDescendng()
  {
    var result = _testRecords.CreateAuthorRankingSorted();
    Assert.Equal("Joe", result.First().Author);
    Assert.Equal(9, result.First().FileTouches);
    Assert.Equal("Anna", result.Second().Author);
    Assert.Equal(3, result.Second().FileTouches);
  }
}
