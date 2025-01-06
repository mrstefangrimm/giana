using FluentAssertions;
using Giana.Api.Core.Fluent;

namespace Giana.Api.Core.Tests;

public class FluentRenameTest : FluentTestBase
{
  [Fact]
  public void RenameAuthor_KnownAuthors_ChangesAuthorsRecords()
    => _testRecords
    .Rename()
    .Author("Anna", "Joe")
    .Build().Value
    .Should().NotContain(item => item.Author == "Joe").And.OnlyContain(item => item.Author == "Anna");

  [Fact]
  public void RenameAuthor_UnknownNewAuthor_RenamesCurrentAuthorsRecords()
    => _testRecords
    .Rename()
    .Author("Jim", "Joe")
    .Build().Value
    .Should().NotContain(item => item.Author == "Joe").And.Contain(item => item.Author == "Jim").And.Contain(item => item.Author == "Anna");

  [Fact]
  public void RenameAuthor_UnknownAuthor_ReturnsCurrentAuthorsRecords()
    => _testRecords
    .Rename()
    .Author("Anna", "Jim")
    .Build().Value
    .Should().Contain(item => item.Author == "Anna").And.Contain(item => item.Author == "Joe");
}
