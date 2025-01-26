using Giana.App.Shared;
using System.IO;
using System.Threading.Tasks;

namespace Giana.Api.Concurrency;

public class AppActionsExecute
{
  private const string gitExePath = @"C:\Program Files\Git\bin\git.exe";
  private const string gitRepositoryGiana = "https://github.com/mrstefangrimm/giana.git";
  private const string gitRepositoryGrpc = "https://github.com/mrstefangrimm/Modern-API-Design-with-gRPC-CSharp.git";
  private const string gitRepositoryPhaso = "https://github.com/mrstefangrimm/Phaso.git";
  private const string gitRepositoryParc = "https://github.com/mrstefangrimm/Parc.git";
  private const string gitRepositoryTSM = "https://github.com/mrstefangrimm/TemplateStateMachine.git";
  private const string gitRepositoryCollares = "https://github.com/mrstefangrimm/Collares.git";

  [Fact]
  public void ExecuteAsync_GianaCalledInParallel_WithSameResult()
  {
    var query = new Query
    {
      Sources = [gitRepositoryGiana],
      Analyzer = "author-ranking",
      OutputFormat = "csv",
    };

    var rouines = new Routine[100];
    var tasks = new Task[rouines.Length];

    for (int i = 0; i < rouines.Length; i++)
    {
      var routine = query.CreateRoutine(new StringWriter());

      rouines[i] = routine;
      tasks[i] = App.Shared.Actions.ExecuteAsync(routine, () => gitExePath, 60000);
    }

    Task.WaitAll(tasks);

    for (int i = 1; i < rouines.Length; i++)
    {
      var result1 = rouines[i - 1].OutputWriter.ToString();
      var result2 = rouines[i].OutputWriter.ToString();
      Assert.Equal(result1, result2);
    }

    foreach (var r in rouines)
    {
      r.OutputWriter.Close();
    }
  }

    [Fact]
  public void ExecuteAsync_ManyReposCalledInParallel_WithCorrectFormat()
  {
    var urls = new string[]
    {
      gitRepositoryGiana,
      gitRepositoryGrpc,
      gitRepositoryPhaso,
      gitRepositoryParc,
      gitRepositoryTSM,
      gitRepositoryCollares,
    };
    var queries = new Query[urls.Length];
    queries[0] = new Query { Sources = [urls[0]], Analyzer = "author-ranking", OutputFormat = "csv" };
    queries[1] = new Query { Sources = [urls[1]], Analyzer = "author-ranking", OutputFormat = "csv" };
    queries[2] = new Query { Sources = [urls[2]], Analyzer = "author-ranking", OutputFormat = "csv" };
    queries[3] = new Query { Sources = [urls[3]], Analyzer = "author-ranking", OutputFormat = "csv" };
    queries[4] = new Query { Sources = [urls[4]], Analyzer = "author-ranking", OutputFormat = "csv" };
    queries[5] = new Query { Sources = [urls[5]], Analyzer = "author-ranking", OutputFormat = "csv" };

    var rouines = new Routine[urls.Length];
    var tasks = new Task[urls.Length];

    for (int i = 0; i < queries.Length; i++)
    {
      var routine = queries[i].CreateRoutine(new StringWriter());

      rouines[i] = routine;
      tasks[i] = App.Shared.Actions.ExecuteAsync(routine, () => gitExePath, 60000);
    }

    Task.WaitAll(tasks);

    for (int i = 0; i < rouines.Length; i++)
    {
      var result = rouines[i].OutputWriter.ToString();
      Assert.StartsWith("Author,FileTouches", result);
      Assert.Contains("mrstefangrimm", result);
    }

    foreach (var r in rouines)
    {
      r.OutputWriter.Close();
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
