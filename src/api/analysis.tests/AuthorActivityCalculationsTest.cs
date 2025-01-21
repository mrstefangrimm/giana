using Giana.Api.Analysis.Activity;
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
    Assert.Equal("Anna", lines.ToArray().Second().Split(',').First());
    Assert.Equal("2", lines.ToArray().Second().Split(',').Second());
    Assert.Equal("0", lines.ToArray().Second().Split(',').Last());
    Assert.Equal("Joe", lines.ToArray().Third().Split(',').First());
    Assert.Equal("6", lines.ToArray().Third().Split(',').Second());
    Assert.Equal("1", lines.ToArray().Third().Split(',').Last());
  }
}

static class Extensions
{
  public static string Second(this string[] source)
  {
    return source[1];
  }

  public static string Third(this string[] source)
  {
    return source[2];
  }
}
