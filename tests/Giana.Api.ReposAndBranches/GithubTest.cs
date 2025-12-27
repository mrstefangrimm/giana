using Giana.Api.Analysis.Ranking;
using Giana.Api.Core;
using Giana.Api.Load;
using System;
using System.Threading.Tasks;

namespace Giana.Api.ReposAndBranches;

public class GithubTest
{
  private const string gitExePath = @"C:\Program Files\Git\bin\git.exe";

  [Theory]
  [InlineData(null, 22)]
  [InlineData("release/9.0", 6)]
  public async Task Commits_InNovember2025_OnBranchHaveCount(string branch, int commits)
  {
    // https://github.com/dotnet/wpf/commits/main/?since=2025-11-01&until=2025-12-01
    // https://github.com/dotnet/wpf/commits/release/9.0/?since=2025-11-01&until=2025-12-01

    const string gitRepository = "https://github.com/dotnet/wpf.git";
    DateTime entriesFrom = new DateTime(2025, 11, 01, 0, 0, 0, DateTimeKind.Utc);
    DateTime entriesTo = new DateTime(2025, 12, 01, 0, 0, 0, DateTimeKind.Utc);
    DateTime commitsSince = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);

    var repository = await GitRepository.CreateFromBranchAsync(gitExePath, gitRepository, branch);

    var records = await repository.LogAsync(commitsSince);
    records = records.WithTimeRange(entriesFrom, entriesTo);

    var commitRanking = CommitRankingCalculations.CreateCommitRankingSorted(records);

    Assert.Equal(commits, commitRanking.Count);
  }
}
