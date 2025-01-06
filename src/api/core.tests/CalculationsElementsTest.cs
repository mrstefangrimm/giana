using FluentAssertions;

namespace Giana.Api.Core.Tests;

public class CalculationsElementsTest : CalculationsTestBase
{
  [Fact]
  public void WithElements_Skip0Take9_ReturnsAllRecords()
    => _testRecords.WithElements(0, 9).Should().HaveCount(_testRecords.Count);

  [Fact]
  public void WithElements_Skip4Take2_ReturnsSecondCommitElements()
    => _testRecords.WithElements(4, 2)
    .Should().Contain(item => item.Message == "Second commit.")
    .And.NotContain(item => item.Message != "Second commit.");
}