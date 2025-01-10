using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;

namespace Giana.Api.Core.Tests;

public class FluentTestBase
{
  protected static readonly IFormatProvider _fmt = new CultureInfo("en-US");
  protected readonly IImmutableList<GitLogRecord> _testRecords;

  protected FluentTestBase()
  {
    _testRecords = GitLogData().ToImmutableList();
  }

  protected static IEnumerable<GitLogRecord> GitLogData()
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