﻿using FluentAssertions;
using System;
using System.Linq;
using static Giana.App.Shared.Calculations;

namespace Giana.App.Shared.Tests;

public class CalculationsTest : AppSharedTestBase
{
  [Fact]
  public void Name_WithDefaultParameter_ThisMethodIsReturned()
  {
    var name = Name();
    Assert.Equal("Name_WithDefaultParameter_ThisMethodIsReturned", name);
  }

  [Fact]
  public void CreateRoutine_WhenSourcesOrAnalyzerIsNull_ArgumentNullExceptionIsThrown()
  {
    var query = new Query();

    Assert.Throws<ArgumentNullException>(() => query.CreateRoutine(Console.Out));

    query.Sources = ["https://test"];
    Assert.Throws<ArgumentNullException>(() => query.CreateRoutine(Console.Out));

    query.Analyzer = "file-ranking";
    Assert.Throws<ArgumentNullException>(() => query.CreateRoutine(Console.Out));

    query.OutputFormat = "csv";
    Assert.Throws<ArgumentNullException>(() => query.CreateRoutine(null));

    Assert.NotNull(query.CreateRoutine(Console.Out));
  }

  [Fact]
  public void CreateRoutine_TimeRangesAreUsed_ThenResultsAreFilteredByTimeRanges()
  {
    var query = new Query();
    query.Sources = ["https://test"];
    query.Analyzer = "file-ranking";
    query.OutputFormat = "csv";
    query.TimeRanges = [
      new TimePeriod()
      {
        Begin = DateTime.Parse("2024-12-20T18:00:00Z", _fmt), End =  DateTime.Parse("2024-12-20T20:00:00Z", _fmt)
      },
      new TimePeriod()
      {
        Begin = DateTime.Parse("2025-02-01T00:00:00Z", _fmt), End =  DateTime.Parse("2025-02-10T10:00:00Z", _fmt)
      }];

    var routine = query.CreateRoutine(Console.Out);

    var result = _testRecords.Where(x => routine.TimeRanges.Any(tp => tp.Begin <= x.Date && x.Date <= tp.End)).ToArray();

    result
      .Should()
      .Contain(x => x.Commit == "abc" || x.Commit == "cde")
      .And
      .NotContain(x => x.Commit == "bcd");
  }

  [Fact]
  public void CreateRoutine_CommitsFromIsUsed_ThenResultsAreFilteredByCommitsFrom()
  {
    var query = new Query();
    query.Sources = ["https://test"];
    query.Analyzer = "file-ranking";
    query.OutputFormat = "csv";
    query.CommitsFrom = DateTime.Parse("2024-12-21T10:00:00Z", _fmt);

    var routine = query.CreateRoutine(Console.Out);

    var result = _testRecords.Where(x => x.Date > routine.CommitsFrom).ToArray();

    result
      .Should()
      .Contain(x => x.Commit == "bcd" || x.Commit == "cde")
      .And
      .NotContain(x => x.Commit == "abc");
  }
}
