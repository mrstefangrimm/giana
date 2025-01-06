using FluentAssertions;
using Giana.Api.Shared.Fluent;

namespace Giana.Api.Shared.Tests;

public class FluentTimePeriodTest : FluentTestBase
{
  [Fact]
  public void IncludeTimePeriod_BeginMinEndMax_ReturnsAllRecords()
    => _testRecords
    .TimePeriod()
    .TimePeriod(DateTime.MinValue, DateTime.MaxValue)
    .Build().Value
    .Should().HaveCount(_testRecords.Count);

  [Fact]
  public void IncludeTimePeriod_BeginMinEndWithinRecords_ReturnsAllUntilEnd()
    => _testRecords
    .TimePeriod()
    .TimePeriod(DateTime.MinValue, DateTime.Parse("2024-12-21T11:00:00Z", _fmt))
    .Build().Value
    .Should().Contain(item => item.Commit == "abc" || item.Commit == "bcd")
    .And.NotContain(item => item.Commit == "cde");
}