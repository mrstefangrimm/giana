using Giana.Api.Analysis.Ranking;
using Giana.Api.Core;
using Giana.Api.Load;

namespace Giana.Api.ReposAndBranches;

public class GitlabTest
{
  private static readonly string GitExePath =
    OperatingSystem.IsWindows()
      ? @"C:\Program Files\Git\bin\git.exe"
      : "/usr/bin/git";

  [Theory]
  [InlineData(null, 47)]
  [InlineData("REL_2.18.0.0_EXTERNAL", 40)]
  public async Task Commits_InOctober2025_OnBranchHaveCount(string branch, int commits)
  {
    // https://gitlab.com/tortoisegit/tortoisegit/-/commits/master
    // https://gitlab.com/tortoisegit/tortoisegit/-/commits/REL_2.18.0.0_EXTERNAL

    const string gitRepository = "https://gitlab.com/tortoisegit/tortoisegit.git";
    DateTime entriesFrom = new DateTime(2025, 10, 01, 0, 0, 0, DateTimeKind.Utc);
    DateTime entriesTo = new DateTime(2025, 11, 01, 0, 0, 0, DateTimeKind.Utc);
    DateTime commitsSince = new DateTime(2020, 01, 01, 0, 0, 0, DateTimeKind.Utc);

    var repository = await GitRepository.CreateFromBranchAsync(GitExePath, gitRepository, branch, TestContext.Current.CancellationToken);

    var records = await repository.LogAsync(commitsSince);
    records = records.WithTimeRange(entriesFrom, entriesTo);

    var commitRanking = CommitRankingCalculations.CreateCommitRankingSorted(records);

    Assert.Equal(commits, commitRanking.Count);
  }
}
