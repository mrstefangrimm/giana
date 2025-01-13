using FluentAssertions;
using Giana.Api.Core.Fluent;
using System.Text.RegularExpressions;

namespace Giana.Api.Core.Tests;

public class FluentIncludeExcludeTest : FluentTestBase
{
  [Fact]
  public void IncludeName_KnownName_ReturnsNamesRecords()
  => _testRecords
  .Include()
  .Name(new Regex(".*File.*A.cs"))
  .Build()
  .Should().Contain(item => item.Name == "File0A.cs" || item.Name == "Folder1/File1A.cs" || item.Name == "Folder2/Folder21/File21A.cs");

  [Fact]
  public void IncludeName_UnknownName_ReturnsEmptyList()
    => _testRecords
    .Include()
    .Name(new Regex(".*Dir.*"))
    .Build()
    .Should().BeEmpty();

  [Fact]
  public void ExcludeName_KnownName_ReturnsAllButRecordsWithName()
    => _testRecords
    .Exclude()
    .Name(new Regex("Folder.*"))
    .Build()
    .Should().Contain(item => item.Name.StartsWith("File"));

  [Fact]
  public void ExcludeName_UnknownName_ReturnsAllRecords()
    => _testRecords
    .Exclude()
    .Name(new Regex(".*Dir.*"))
    .Build()
    .Should().HaveCount(_testRecords.Count);

  [Fact]
  public void IncludeCommit_KnownCommit_ReturnsCommitsRecords()
  => _testRecords
  .Include()
  .Commit("bcd")
  .Build()
  .Should().OnlyContain(item => item.Commit == "bcd");

  [Fact]
  public void IncludeCommit_UnknownCommit_ReturnsEmptyList()
    => _testRecords
    .Include()
    .Commit("xyz")
    .Build()
    .Should().BeEmpty();

  [Fact]
  public void ExcludeCommit_KnownCommit_ReturnsAllButCommitsRecords()
    => _testRecords
    .Exclude()
    .Commit("abc")
    .Build()
    .Should().NotContain(item => item.Commit == "abc")
    .And.Contain(item => item.Commit == "bcd" || item.Commit == "cde");

  [Fact]
  public void ExcludeCommit_UnknownCommit_ReturnsAllRecords()
    => _testRecords
    .Exclude()
    .Commit("xyz")
    .Build()
    .Should().HaveCount(_testRecords.Count);

  [Fact]
  public void IncludeAuthor_KnownAuthor_ReturnsAuthorsRecords()
    => _testRecords
    .Include()
    .Author("Joe")
    .Build()
    .Should().OnlyContain(item => item.Author == "Joe");

  [Fact]
  public void IncludeAuthor_UnknownAuthor_ReturnsEmptyList()
    => _testRecords
    .Include()
    .Author("Jim")
    .Build()
    .Should().BeEmpty();

  [Fact]
  public void ExcludeAuthor_KnownAuthor_ReturnsAllButAuthorsRecords()
    => _testRecords
    .Exclude()
    .Author("Anna")
    .Build()
    .Should().NotContain(item => item.Author == "Anna")
    .And.Contain(item => item.Author == "Joe");

  [Fact]
  public void ExcludeAuthor_UnknownAuthor_ReturnsAllRecords()
    => _testRecords
    .Exclude()
    .Author("Jim")
    .Build()
    .Should().HaveCount(_testRecords.Count);

  [Fact]
  public void IncludeMessage_KnownMessage_ReturnsRecordsWithMessage()
  => _testRecords
    .Include()
    .Message(new Regex("^(First|Third).*"))
    .Build()
    .Should().Contain(item => item.Commit == "abc" || item.Commit == "cde")
    .And.NotContain(item => item.Commit == "bcd");

  [Fact]
  public void IncludeMessage_UnknownMessage_ReturnsEmptyList()
    => _testRecords
    .Include()
    .Message(new Regex("^Forth.*"))
    .Build()
    .Should().BeEmpty();

  [Fact]
  public void ExcludeMessage_KnownMessage_ReturnsAllButRecordsWithMessage()
    => _testRecords
    .Exclude()
    .Message(new Regex("^(Second|Third).*"))
    .Build()
    .Should().Contain(item => item.Commit == "abc")
    .And.NotContain(item => item.Commit == "bcd" || item.Commit == "cde");

  [Fact]
  public void ExcludeMessage_UnknownMessage_ReturnsAllRecords()
    => _testRecords
    .Exclude()
    .Message(new Regex("^Forth.*"))
    .Build()
    .Should().HaveCount(_testRecords.Count);
}