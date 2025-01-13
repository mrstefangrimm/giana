using FluentAssertions;
using Giana.Api.Core.Fluent;
using System;

namespace Giana.Api.Core.Tests;

public class FluentCombinedTest : FluentTestBase
{
  [Fact]
  public void TimeRange_Elements_ReturnsSecondCommitElements()
    => _testRecords
    .TimeRange().In(DateTime.Parse("2024-12-20T19:35:00Z", _fmt), DateTime.Parse("2024-12-21T18:00:00Z", _fmt))
    .Elements().In(4, 2)
    .Build()
    .Should().Contain(x => x.Message == "Second commit.")
    .And.NotContain(x => x.Message != "Second commit.");

  [Fact]
  public void TimeRange_RenameAndExclude_ReturnsSecondCommitElements()
    => _testRecords
    .TimeRange()
    .In(DateTime.Parse("2024-12-20T19:35:00Z", _fmt), DateTime.Parse("2024-12-21T18:00:00Z", _fmt))
    .Rename().Author("John", "Joe")
    .Exclude().Author("John")
    .Build()
    .Should().Contain(x => x.Message == "Second commit.")
    .And.NotContain(x => x.Message != "Second commit.");

  [Fact]
  public void Include_Elements_ReturnsSecondCommitFirstElement()
    => _testRecords
    .Include().Commit("bcd")
    .Elements().In(0, 1)
    .Build()
    .Should().HaveCount(1)
    .And.Contain(x => x.Name == "File0A.cs");
}