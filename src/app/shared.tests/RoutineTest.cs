using Giana.Api.Analysis;
using System;
using System.IO;
using System.Threading;

namespace Giana.App.Shared.Tests;

public class RoutineTest : AppSharedTestBase
{
  [Fact]
  public void Analyze_WhenFileRanking_ThenFirstLineIsfileRankingHeader()
  {
    var query = new Query();
    query.Sources = ["https://test"];
    query.Analyzer = "file-ranking";
    query.OutputFormat = "csv";

    using var writer = new StringWriter();
    var context = new Api.Analysis.ExecutionContext(_testRecords, _activeNames, "csv", writer, CancellationToken.None);

    var routine = query.CreateRoutine(Console.Out);
    routine.Analyze(context);

    var result = writer.ToString();
    Assert.StartsWith("Repository,FileName,ChangeCount", result);
  }

  [Fact]
  public void Analyze_WhenCommitRanking_ThenFirstLineIsCommitRankingHeader()
  {
    var query = new Query();
    query.Sources = ["https://test"];
    query.Analyzer = "commit-ranking";
    query.OutputFormat = "csv";

    using var writer = new StringWriter();
    var context = new Api.Analysis.ExecutionContext(_testRecords, _activeNames, "csv", writer, CancellationToken.None);

    var routine = query.CreateRoutine(Console.Out);
    routine.Analyze(context);

    var result = writer.ToString();
    Assert.StartsWith("Repository,Commit,CommitedFiles,Commit Message", result);
  }

  [Fact]
  public void Analyze_WhenAuthorRanking_ThenFirstLineIsAuthorRankingHeader()
  {
    var query = new Query();
    query.Sources = ["https://test"];
    query.Analyzer = "author-ranking";
    query.OutputFormat = "csv";

    using var writer = new StringWriter();
    var context = new Api.Analysis.ExecutionContext(_testRecords, _activeNames, "csv", writer, CancellationToken.None);

    var routine = query.CreateRoutine(Console.Out);
    routine.Analyze(context);

    var result = writer.ToString();
    Assert.StartsWith("Author,FileTouches", result);
  }

  [Fact]
  public void Analyze_WhenFileCoupling_ThenFirstLineIsFileCouplingHeader()
  {
    var query = new Query();
    query.Sources = ["https://test"];
    query.Analyzer = "file-coupling";
    query.OutputFormat = "csv";

    using var writer = new StringWriter();
    var context = new Api.Analysis.ExecutionContext(_testRecords, _activeNames, "csv", writer, CancellationToken.None);

    var routine = query.CreateRoutine(Console.Out);
    routine.Analyze(context);

    var result = writer.ToString();
    Assert.StartsWith("FileName1,FileName2,CouplingCount", result);
  }

  [Fact]
  public void Analyze_WhenFolderCouplingAndCohesion_ThenFirstLineIsFolderCouplingAndCohesionHeader()
  {
    var query = new Query();
    query.Sources = ["https://test"];
    query.Analyzer = "folder-coupling-and-cohesion";
    query.OutputFormat = "csv";

    using var writer = new StringWriter();
    var context = new Api.Analysis.ExecutionContext(_testRecords, _activeNames, "csv", writer, CancellationToken.None);

    var routine = query.CreateRoutine(Console.Out);
    routine.Analyze(context);

    var result = writer.ToString();
    Assert.StartsWith("FolderName,CohesionCount,CouplingCount,Ratio Cohesion/Coupling", result);
  }

  [Fact]
  public void Analyze_WhenProjectCouplingAndCohesion_ThenFirstLineIsProjectCouplingAndCohesionHeader()
  {
    var query = new Query();
    query.Sources = ["https://test"];
    query.Analyzer = "project-coupling-and-cohesion";
    query.OutputFormat = "csv";

    using var writer = new StringWriter();
    var context = new Api.Analysis.ExecutionContext(_testRecords, _activeNames, "csv", writer, CancellationToken.None);

    var routine = query.CreateRoutine(Console.Out);
    routine.Analyze(context);

    var result = writer.ToString();
    Assert.StartsWith("ProjectName,CohesionCount,CouplingCount,Ratio Cohesion/Coupling", result);
  }

  [Fact]
  public void Analyze_WhenAuthorActivity_ThenFirstLineIsYearsAndWeeks()
  {
    var query = new Query();
    query.Sources = ["https://test"];
    query.Analyzer = "author-activity";
    query.OutputFormat = "csv";

    using var writer = new StringWriter();
    var context = new Api.Analysis.ExecutionContext(_testRecords, _activeNames, "csv", writer, CancellationToken.None);

    var routine = query.CreateRoutine(Console.Out);
    routine.Analyze(context);

    var result = writer.ToString();
    Assert.StartsWith(",2024-51,2024-52,2024-01,2025-01,2025-02,2025-03,2025-04,2025-05", result);
  }
}
