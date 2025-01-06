using FluentAssertions;
using Giana.Api.Core.Fluent;

namespace Giana.Api.Core.Tests;

public class FluentElementsRangeTest : FluentTestBase
{
  [Fact]
  public void WithElements_Skip0Take9_ReturnsAllRecords()
    => _testRecords
    .Elements()
    .In(0, 9)
    .Build().Value
    .Should().HaveCount(_testRecords.Count);

  [Fact]
  public void WithElements_Skip4Take2_ReturnsSecondCommitElements()
    => _testRecords
    .Elements()
    .In(4, 2)
    .Build().Value
    .Should().Contain(item => item.Message == "Second commit.")
    .And.NotContain(item => item.Message != "Second commit.");
}