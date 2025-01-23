using Giana.Api.Analysis.Ranking;
using System.Linq;

namespace Giana.Api.Analysis.Tests;

public class CommitRankingCalculationsTest : CalculationsTestBase
{
  [Fact]
  public void CreateCommitRankingSorted_WithTestRecords_SortedByCommittedFilesDescendng()
  {
    var result = _testRecords.CreateCommitRankingSorted();
    Assert.Equal(6, result.First().CommittedFiles);
    Assert.Equal(3, result.Second().CommittedFiles);
    Assert.Equal(3, result.Third().CommittedFiles);
  }
}
