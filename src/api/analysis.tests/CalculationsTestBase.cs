using Giana.Api.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;

namespace Giana.Api.Analysis.Tests;

public class CalculationsTestBase
{
  protected static readonly IFormatProvider _fmt = new CultureInfo("en-US");
  protected readonly IImmutableList<GitLogRecord> _testRecords;
  protected readonly IImmutableList<string> _activeNames;

  protected CalculationsTestBase()
  {
    _testRecords = GitLogData().ToImmutableList();
    _activeNames = new List<string>(["File0A.cs", "Folder1/File1A.cs", "Folder2/Folder21/File21A.cs", "Folder2/Folder21/File21B.cs"]).ToImmutableList();
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
    yield return new GitLogRecord("Gina", "Folder1/File1A.cs", "cde", "Joe", "Third commit.", DateTime.Parse("2025-02-01T10:00:00Z", _fmt));
    // yield return new GitLogRecord("Gina", "Properties/launchSettings.json", "cde", "Joe", "Third commit.", DateTime.Parse("2025-02-01T10:00:00Z", _fmt));
    // yield return new GitLogRecord("Gina", "Folder1/Folder1.csproj", "cde", "Joe", "Third commit.", DateTime.Parse("2025-02-01T10:00:00Z", _fmt));
    // yield return new GitLogRecord("Gina", "Folder2/Folder21/Folder21.csproj", "cde", "Joe", "Third commit.", DateTime.Parse("2025-02-01T10:00:00Z", _fmt));
  }

}
