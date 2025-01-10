using FluentAssertions;
using System;

namespace Giana.Api.Core.Tests;

public class CalculationsTimeRangeTest : CalculationsTestBase
{
  [Fact]
  public void WithTimeRange_BeginMinEndMax_ReturnsAllRecords()
    => _testRecords.WithTimeRange(DateTime.MinValue, DateTime.MaxValue).Should().HaveCount(_testRecords.Count);

  [Fact]
  public void WithTimeRange_BeginMinEndWithinRecords_ReturnsAllUntilEnd()
    => _testRecords.WithTimeRange(DateTime.MinValue, DateTime.Parse("2024-12-21T11:00:00Z", _fmt))
    .Should().Contain(item => item.Commit == "abc" || item.Commit == "bcd")
    .And.NotContain(item => item.Commit == "cde");
}