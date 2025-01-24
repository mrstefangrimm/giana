using Giana.Api.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;

namespace Giana.App.Shared.Tests;

public class AppSharedTestBase
{
  protected static readonly IFormatProvider _fmt = new CultureInfo("en-US");
  protected readonly IImmutableList<GitLogRecord> _testRecords;
  protected readonly IImmutableList<string> _activeNames;

  protected AppSharedTestBase()
  {
    _testRecords = GitLogData().ToImmutableList();
    _activeNames = new List<string>(
      [
      "Readme.md",
      "Folder1/Folder1.csproj",
      "Folder1/File1A.cs",
      "Folder2/Folder2.csproj",
      "Folder2/Folder21/File21A.cs",
      "Folder2/Folder21/File21B.cs",
      "Properties/launchSettings.json"
    ]).ToImmutableList();
  }

  /// <summary>
  /// commit abc by Joe, 2024-12-20
  ///   Readme.md
  ///   Folder1/Folder1.csproj
  ///   Folder1/File1A.cs
  ///   Folder2/Folder2.csproj
  ///   Folder2/Folder21/File21A.cs
  ///   Folder2/Folder21/File21B.cs
  ///
  /// commit bcd by Anna, 2024-12-21
  ///   Readme.md
  ///   Contribute.md
  ///   Properties/launchSettings.json
  /// 
  ///  commit cde by Joe, 2025-02-01
  ///    Readme.md
  ///    Contribute.md
  ///    Folder1/File1A.cs
  /// </summary>
  /// <returns></returns>
  protected static IEnumerable<GitLogRecord> GitLogData()
  {
    yield return new GitLogRecord("Gina", "Readme.md", "abc", "Joe", "First commit.", DateTime.Parse("2024-12-20T19:35:00Z", _fmt));
    yield return new GitLogRecord("Gina", "Folder1/Folder1.csproj", "abc", "Joe", "First commit.", DateTime.Parse("2024-12-20T19:35:00Z", _fmt));
    yield return new GitLogRecord("Gina", "Folder1/File1A.cs", "abc", "Joe", "First commit.", DateTime.Parse("2024-12-20T19:35:00Z", _fmt));
    yield return new GitLogRecord("Gina", "Folder2/Folder2.csproj", "abc", "Joe", "First commit.", DateTime.Parse("2024-12-20T19:35:00Z", _fmt));
    yield return new GitLogRecord("Gina", "Folder2/Folder21/File21A.cs", "abc", "Joe", "First commit.", DateTime.Parse("2024-12-20T19:35:00Z", _fmt));
    yield return new GitLogRecord("Gina", "Folder2/Folder21/File21B.cs", "abc", "Joe", "First commit.", DateTime.Parse("2024-12-20T19:35:00Z", _fmt));

    yield return new GitLogRecord("Gina", "Readme.md", "bcd", "Anna", "Second commit.", DateTime.Parse("2024-12-21T18:00:00Z", _fmt));
    yield return new GitLogRecord("Gina", "Contribute.md", "bcd", "Anna", "Second commit.", DateTime.Parse("2024-12-21T18:00:00Z", _fmt));
    yield return new GitLogRecord("Gina", "Properties/launchSettings.json", "bcd", "Anna", "Second commit.", DateTime.Parse("2024-12-21T18:00:00Z", _fmt));

    yield return new GitLogRecord("Gina", "Readme.md", "cde", "Joe", "Third commit.", DateTime.Parse("2025-02-01T10:00:00Z", _fmt));
    yield return new GitLogRecord("Gina", "Contribute.md", "cde", "Joe", "Third commit.", DateTime.Parse("2025-02-01T10:00:00Z", _fmt));
    yield return new GitLogRecord("Gina", "Folder1/File1A.cs", "cde", "Joe", "Third commit.", DateTime.Parse("2025-02-01T10:00:00Z", _fmt));
  }

}
