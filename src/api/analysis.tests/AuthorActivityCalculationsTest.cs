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
    Assert.Equal("Anna", lines.Second().Split(',').First());
    Assert.Equal("3", lines.Second().Split(',').Second());
    Assert.Equal("0", lines.Second().Split(',').Last());
    Assert.Equal("Joe", lines.Third().Split(',').First());
    Assert.Equal("6", lines.Third().Split(',').Second());
    Assert.Equal("3", lines.Third().Split(',').Last());
  }
}
