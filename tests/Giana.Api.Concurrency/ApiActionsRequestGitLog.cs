using Giana.Api.Core;
using Giana.Api.Load;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Giana.Api.Concurrency;

public class ApiActionsRequestGitLog
{
  private const string gitExePath = @"C:\Program Files\Git\bin\git.exe";
  private const string gitRepository = "https://github.com/mrstefangrimm/giana.git";

  [Fact]
  public void RequestGitLog_CalledInParallel_WithSameResult()
  {
    var localRepo = Actions.CreateCloneFromUri(gitRepository, gitExePath);
    var repoName = Actions.RequestRepositoryName(localRepo, gitExePath);
    using var defer = new Defer(() =>
    {
      RemoveReadOnly(localRepo);
      Directory.Delete(localRepo, true);
    });

    var tasks = new Task<IImmutableList<GitLogRecord>>[1000];
    for (int i = 0; i < tasks.Length; i++)
    {
      tasks[i] = Actions.RequestGitLogAsync(localRepo, repoName, gitExePath, CancellationToken.None);
    }

    Task.WaitAll(tasks);

    for (int i = 1; i < tasks.Length; i++)
    {
      var result1 = tasks[i-1].Result;
      var result2 = tasks[i].Result;
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
