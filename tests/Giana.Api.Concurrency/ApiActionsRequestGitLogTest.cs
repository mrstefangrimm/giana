using Giana.Api.Core;
using Giana.Api.Load;
using System.Collections.Immutable;

namespace Giana.Api.Concurrency;

public class ApiActionsRequestGitLogTest
{
  private static readonly string GitExePath =
    OperatingSystem.IsWindows()
      ? @"C:\Program Files\Git\bin\git.exe"
      : "/usr/bin/git";   
  private const string GitRepository = "https://github.com/mrstefangrimm/giana.git";

  [Fact]
  public async Task RequestGitLog_CalledInParallel_WithSameResult()
  {
    var localRepo = await Actions.CreateCloneFromUriAsync(GitExePath, GitRepository, TestContext.Current.CancellationToken);
    var repoName = await Actions.RequestRepositoryNameAsync(GitExePath, localRepo, TestContext.Current.CancellationToken);
    using var defer = new Defer(() =>
    {
      RemoveReadOnly(localRepo);
      Directory.Delete(localRepo, true);
    });

    var tasks = new Task<ImmutableList<GitLogRecord>>[1000];
    var commitsSince = new DateTime(2020, 01, 01);
    for (int i = 0; i < tasks.Length; i++)
    {
      tasks[i] = Actions.RequestGitLogAsync(GitExePath, repoName, localRepo, commitsSince, TestContext.Current.CancellationToken);
    }

    await Task.WhenAll(tasks);

    for (int i = 1; i < tasks.Length; i++)
    {
      var result1 = await tasks[i-1];
      var result2 = await tasks[i];
      Assert.Equal(result1, result2);
    }
  }

  private static void RemoveReadOnly(string dir)
  {
    var subDirs = Directory.GetDirectories(dir);
    foreach (var subDir in subDirs)
    {
      RemoveReadOnly(subDir);
    }
    var files = Directory.GetFiles(dir);
    foreach (var file in files)
    {
      File.SetAttributes(file, File.GetAttributes(file) & ~FileAttributes.ReadOnly);
    }
  }

}
