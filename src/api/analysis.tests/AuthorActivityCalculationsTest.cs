using Giana.Api.Analysis.Activity;
using System;
using System.Linq;

namespace Giana.Api.Analysis.Tests;

public class AuthorActivityCalculationsTest : CalculationsTestBase
{
  [Fact]
  public void CreateActivityChartAsCsv_WithTestRecords_CsvLinesAreReturned()
  {
    var lines = _testRecords.CreateActivityChartAsCsv();
    Assert.Equal("2024-51", lines.First().Split(',').Second());
    Assert.Equal("2025-05", lines.First().Split(',').Last());
    Assert.Equal("Anna", lines.Second().Split(',').First());
    Assert.Equal("3", lines.Second().Split(',').Second());
    Assert.Equal("0", lines.Second().Split(',').Last());
    Assert.Equal("Joe", lines.Third().Split(',').First());
    Assert.Equal("6", lines.Third().Split(',').Second());
    Assert.Equal("3", lines.Third().Split(',').Last());
  }

  [Fact]
  public void CreateListOfWeeks_WithYearChangeMondayToFriday_YearAndWeekNumberIsCorrect()
  {
    var weeks = AuthorActivityCalculations.CreateListOfWeeks(new DateTime(2025, 12, 22), new DateTime(2026, 1, 9));

    Assert.Equal(3, weeks.Count());
    Assert.Equal("2025-52", weeks[0]);
    Assert.Equal("2026-01", weeks[1]);
    Assert.Equal("2026-02", weeks[2]);
  }

  [Fact]
  public void CreateListOfWeeks_WithYearChangeThursdayToThursday_YearAndWeekNumberIsCorrect()
  {
    var weeks = AuthorActivityCalculations.CreateListOfWeeks(new DateTime(2025, 12, 25), new DateTime(2026, 1, 8));

    Assert.Equal(3, weeks.Count());
    Assert.Equal("2025-52", weeks[0]);
    Assert.Equal("2026-01", weeks[1]);
    Assert.Equal("2026-02", weeks[2]);
  }

  [Fact]
  public void CreateListOfWeeks_WithYearChangeFridayToSunday_YearAndWeekNumberIsCorrect()
  {
    var weeks = AuthorActivityCalculations.CreateListOfWeeks(new DateTime(2025, 12, 26), new DateTime(2026, 1, 10));

    Assert.Equal(3, weeks.Count());
    Assert.Equal("2025-52", weeks[0]);
    Assert.Equal("2026-01", weeks[1]);
    Assert.Equal("2026-02", weeks[2]);
  }
}
