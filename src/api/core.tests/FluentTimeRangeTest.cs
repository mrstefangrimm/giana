using FluentAssertions;
using Giana.Api.Core.Fluent;

namespace Giana.Api.Core.Tests;

public class FluentTimeRangeTest : FluentTestBase
{
  [Fact]
  public void TimeRange_BeginMinEndMax_ReturnsAllRecords()
    => _testRecords
    .TimeRange()
    .In(DateTime.MinValue, DateTime.MaxValue)
    .Build().Value
    .Should().HaveCount(_testRecords.Count);

  [Fact]
  public void TimeRange_BeginMinEndWithinRecords_ReturnsAllUntilEnd()
    => _testRecords
    .TimeRange()
    .In(DateTime.MinValue, DateTime.Parse("2024-12-21T11:00:00Z", _fmt))
    .Build().Value
    .Should().Contain(item => item.Commit == "abc" || item.Commit == "bcd")
    .And.NotContain(item => item.Commit == "cde");
}