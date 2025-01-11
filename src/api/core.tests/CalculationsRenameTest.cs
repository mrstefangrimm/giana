using FluentAssertions;

namespace Giana.Api.Core.Tests;

public class CalculationsRenameTest : CalculationsTestBase
{
  [Fact]
  public void RenameAuthor_KnownAuthors_ChangesAuthorsRecords()
    => _testRecords.RenameAuthor("Anna", "Joe").Should().NotContain(item => item.Author == "Joe").And.OnlyContain(item => item.Author == "Anna");

  [Fact]
  public void RenameAuthor_UnknownNewAuthor_RenamesCurrentAuthorsRecords()
    => _testRecords.RenameAuthor("Jim", "Joe").Should().NotContain(item => item.Author == "Joe").And.Contain(item => item.Author == "Jim").And.Contain(item => item.Author == "Anna");

  [Fact]
  public void RenameAuthor_UnknownAuthor_ReturnsCurrentAuthorsRecords()
    => _testRecords.RenameAuthor("Anna", "Jim").Should().Contain(item => item.Author == "Anna").And.Contain(item => item.Author == "Joe");
}