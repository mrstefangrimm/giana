using Giana.Api.Core;
using Giana.Api.Load;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;

namespace Giana.Api.Concurrency;

public class ApiActionsRequestGitLogTest
{
  private const string gitExePath = @"C:\Program Files\Git\bin\git.exe";
  private const string gitRepository = "https://github.com/mrstefangrimm/giana.git";

  [Fact]
  public async Task RequestGitLog_CalledInParallel_WithSameResult()
  {
    var localRepo = await Actions.CreateCloneFromUriAsync(gitExePath, gitRepository);
    var repoName = await Actions.RequestRepositoryNameAsync(gitExePath, localRepo);
    using var defer = new Defer(() =>
    {
      RemoveReadOnly(localRepo);
      Directory.Delete(localRepo, true);
    });

    var tasks = new Task<ImmutableList<GitLogRecord>>[1000];
    for (int i = 0; i < tasks.Length; i++)
    {
      tasks[i] = Actions.RequestGitLogAsync(gitExePath, repoName, localRepo);
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
