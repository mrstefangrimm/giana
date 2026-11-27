using Giana.App.Shared;

namespace Giana.App.Concurrency;

public class AppActionsExecuteTest
{
  private static readonly string GitExePath =
    OperatingSystem.IsWindows()
      ? @"C:\Program Files\Git\bin\git.exe"
      : "/usr/bin/git";
  private const string GitRepositoryGiana = "https://github.com/mrstefangrimm/giana.git";
  private const string GitRepositoryGrpc = "https://github.com/mrstefangrimm/Modern-API-Design-with-gRPC-CSharp.git";
  private const string GitRepositoryPhaso = "https://github.com/mrstefangrimm/Phaso.git";
  private const string GitRepositoryParc = "https://github.com/mrstefangrimm/Parc.git";
  private const string GitRepositoryTsm = "https://github.com/mrstefangrimm/TemplateStateMachine.git";
  private const string GitRepositoryCollares = "https://github.com/mrstefangrimm/Collares.git";

  [Fact]
  public async Task ExecuteAsync_GianaCalledInParallel_WithSameResult()
  {
    var query = new Query
    {
      Sources = [GitRepositoryGiana],
      Analyzer = "author-ranking",
      OutputFormat = "csv",
    };

    var routines = new Routine[100];
    var outputs = new StringWriter[routines.Length];
    var tasks = new Task[routines.Length];

    for (int i = 0; i < routines.Length; i++)
    {
      var routine = query.CreateRoutine();

      routines[i] = routine;
      outputs[i] = new StringWriter();
      tasks[i] = routine.ExecuteAsync(GitExePath, outputs[i], TimeSpan.FromSeconds(60));
    }

    await Task.WhenAll(tasks);

    for (int i = 1; i < routines.Length; i++)
    {
      var result1 = outputs[i - 1].ToString();
      var result2 = outputs[i].ToString();
      Assert.Equal(result1, result2);
    }

    foreach (var writer in outputs)
    {
      writer.Close();
    }
  }

  [Fact]
  public async Task ExecuteAsync_ManyReposCalledInParallel_WithCorrectFormat()
  {
    var urls = new string[]
    {
      GitRepositoryGiana,
      GitRepositoryGrpc,
      GitRepositoryPhaso,
      GitRepositoryParc,
      GitRepositoryTsm,
      GitRepositoryCollares,
    };
    var queries = new Query[urls.Length];
    queries[0] = new Query { Sources = [urls[0]], Analyzer = "author-ranking", OutputFormat = "csv" };
    queries[1] = new Query { Sources = [urls[1]], Analyzer = "author-ranking", OutputFormat = "csv" };
    queries[2] = new Query { Sources = [urls[2]], Analyzer = "author-ranking", OutputFormat = "csv" };
    queries[3] = new Query { Sources = [urls[3]], Analyzer = "author-ranking", OutputFormat = "csv" };
    queries[4] = new Query { Sources = [urls[4]], Analyzer = "author-ranking", OutputFormat = "csv" };
    queries[5] = new Query { Sources = [urls[5]], Analyzer = "author-ranking", OutputFormat = "csv" };

    var routines = new Routine[urls.Length];
    var outputs = new StringWriter[urls.Length];
    var tasks = new Task[urls.Length];

    for (int i = 0; i < queries.Length; i++)
    {
      var routine = queries[i].CreateRoutine();

      routines[i] = routine;
      outputs[i] = new StringWriter();
      tasks[i] = routine.ExecuteAsync(GitExePath, outputs[i], TimeSpan.FromSeconds(60));
    }

    await Task.WhenAll(tasks);

    for (int i = 0; i < routines.Length; i++)
    {
      var result = outputs[i].ToString();
      Assert.StartsWith("Author,FileTouches", result);
      Assert.Contains("mrstefangrimm", result);
    }

    foreach (var writer in outputs)
    {
      writer.Close();
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
