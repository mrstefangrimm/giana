using Giana.Api.Analysis.Ranking;

namespace Giana.Api.Analysis.Tests;

public class CommitRankingCalculationsTest : CalculationsTestBase
{
  [Fact]
  public void CreateCommitRankingSorted_WithTestRecords_SortedByCommittedFilesDescendng()
  {
    var result = _testRecords.CreateCommitRankingSorted();
    Assert.Equal(4, result[0].CommittedFiles);
    Assert.Equal(3, result[1].CommittedFiles);
    Assert.Equal(2, result[2].CommittedFiles);
  }
}
