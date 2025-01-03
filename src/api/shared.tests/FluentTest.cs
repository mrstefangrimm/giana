using FluentAssertions;
using System.Globalization;
using System.Text.RegularExpressions;
using Giana.Api.Shared.Fluent;

namespace Giana.Api.Shared.Tests;

public class FluentTest
{
  private static readonly IFormatProvider _fmt = new CultureInfo("en-US");
  private readonly ICollection<GitLogRecord> _testRecords;

  public FluentTest()
  {
    _testRecords = GitLogData().ToList();
  }

  [Fact]
  public void IncludeAuthor_KnownAuthor_ReturnsAuthorsRecords()
    => _testRecords
    .Include().Author("Joe")
    .Build().Value
    .Should().OnlyContain(item => item.Author == "Joe");

  [Fact]
  public void IncludeAuthor_UnknownAuthor_ReturnsEmptyList()
    => _testRecords
    .Include().Author("Jim")
    .Build().Value
    .Should().BeEmpty();

  [Fact]
  public void ExcludeAuthor_KnownAuthor_ReturnsAllButAuthorsRecords()
    => _testRecords
    .Exclude().Author("Anna")
    .Build().Value
    .Should().NotContain(item => item.Author == "Anna")
    .And.Contain(item => item.Author == "Joe");

  [Fact]
  public void ExcludeAuthor_UnknownAuthor_ReturnsAllRecords()
    => _testRecords
    .Exclude().Author("Jim")
    .Build().Value
    .Should().HaveCount(_testRecords.Count);

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

  [Fact]
  public void IncludeName_KnownName_ReturnsNamesRecords()
    => _testRecords
    .Include()
    .Name(new Regex(".*File.*A.cs"))
    .Build().Value
    .Should().Contain(item => item.Name == "File0A.cs" || item.Name == "Folder1/File1A.cs" || item.Name == "Folder2/Folder21/File21A.cs");

  [Fact]
  public void IncludeName_UnknownName_ReturnsEmptyList()
    => _testRecords
    .Include()
    .Name(new Regex(".*Dir.*"))
    .Build().Value
    .Should().BeEmpty();

  [Fact]
  public void ExcludeName_KnownName_ReturnsAllButRecordsWithName()
    => _testRecords.ExcludeName(new Regex("Folder.*")).Should().Contain(item => item.Name.StartsWith("File"));

  [Fact]
  public void ExcludeName_UnknownName_ReturnsAllRecords()
    => _testRecords.ExcludeName(new Regex(".*Dir.*")).Should().HaveCount(_testRecords.Count);

  [Fact]
  public void IncludeMessage_KnownMessage_ReturnsRecordsWithMessage()
  => _testRecords
    .Include()
    .Message(new Regex("^(First|Third).*"))
    .Build().Value
    .Should().Contain(item => item.Commit == "abc" || item.Commit == "cde")
    .And.NotContain(item => item.Commit == "bcd");

  [Fact]
  public void IncludeMessage_UnknownMessage_ReturnsEmptyList()
    => _testRecords
    .Include()
    .Message(new Regex("^Forth.*"))
    .Build().Value
    .Should().BeEmpty();

  [Fact]
  public void ExcludeName_KnownMessage_ReturnsAllButRecordsWithMessage()
    => _testRecords.ExcludeMessage(new Regex("^(Second|Third).*"))
    .Should().Contain(item => item.Commit == "abc")
    .And.NotContain(item => item.Commit == "bcd" || item.Commit == "cde");

  [Fact]
  public void ExcludeName_UnknownMessage_ReturnsAllRecords()
    => _testRecords.ExcludeMessage(new Regex("^Forth.*")).Should().HaveCount(_testRecords.Count);

  [Fact]
  public void IncludeCommit_KnownCommit_ReturnsCommitsRecords()
    => _testRecords
    .Include()
    .Commit("bcd")
    .Build().Value
    .Should().OnlyContain(item => item.Commit == "bcd");

  [Fact]
  public void IncludeCommit_UnknownCommit_ReturnsEmptyList()
    => _testRecords
    .Include()
    .Commit("xyz")
    .Build().Value
    .Should().BeEmpty();

  [Fact]
  public void ExcludeCommit_KnownCommit_ReturnsAllButCommitsRecords()
    => _testRecords
    .Exclude()
    .Commit("abc")
    .Build().Value
    .Should().NotContain(item => item.Commit == "abc")
    .And.Contain(item => item.Commit == "bcd" || item.Commit == "cde");

  [Fact]
  public void ExcludeCommit_UnknownCommit_ReturnsAllRecords()
    => _testRecords
    .Exclude()
    .Commit("xyz")
    .Build().Value
    .Should().HaveCount(_testRecords.Count);

  [Fact]
  public void IncludeTimePeriod_BeginMinEndMax_ReturnsAllRecords()
    => _testRecords.IncludeTimePeriod(DateTime.MinValue, DateTime.MaxValue).Should().HaveCount(_testRecords.Count);

  [Fact]
  public void IncludeTimePeriod_BeginMinEndWithinRecords_ReturnsAllUntilEnd()
    => _testRecords.IncludeTimePeriod(DateTime.MinValue, DateTime.Parse("2024-12-21T11:00:00Z", _fmt))
    .Should().Contain(item => item.Commit == "abc" || item.Commit == "bcd")
    .And.NotContain(item => item.Commit == "cde");

  private static IEnumerable<GitLogRecord> GitLogData()
  {
    yield return new GitLogRecord("Gina", "File0A.cs", "abc", "Joe", "First commit.", DateTime.Parse("2024-12-20T19:35:00Z", _fmt));
    yield return new GitLogRecord("Gina", "Folder1/File1A.cs", "abc", "Joe", "First commit.", DateTime.Parse("2024-12-20T19:35:00Z", _fmt));
    yield return new GitLogRecord("Gina", "Folder2/Folder21/File21A.cs", "abc", "Joe", "First commit.", DateTime.Parse("2024-12-20T19:35:00Z", _fmt));
    yield return new GitLogRecord("Gina", "Folder2/Folder21/File21B.cs", "abc", "Joe", "First commit.", DateTime.Parse("2024-12-20T19:35:00Z", _fmt));
    yield return new GitLogRecord("Gina", "File0A.cs", "bcd", "Anna", "Second commit.", DateTime.Parse("2024-12-21T18:00:00Z", _fmt));
    yield return new GitLogRecord("Gina", "File0B.cs", "bcd", "Anna", "Second commit.", DateTime.Parse("2024-12-21T18:00:00Z", _fmt));
    yield return new GitLogRecord("Gina", "File0A.cs", "cde", "Joe", "Third commit.", DateTime.Parse("2024-12-22T18:00:00Z", _fmt));
    yield return new GitLogRecord("Gina", "File0B.cs", "cde", "Joe", "Third commit.", DateTime.Parse("2024-12-22T18:00:00Z", _fmt));
    yield return new GitLogRecord("Gina", "Folder1/File1A.cs", "cde", "Joe", "Third commit.", DateTime.Parse("2024-12-22T18:00:00Z", _fmt));
  }
}