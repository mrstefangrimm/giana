using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace Giana.Api.Core;

public static class Calculations
{
  public static ImmutableList<GitLogRecord> IncludeAuthor(this IEnumerable<GitLogRecord> records, string author)
  {
    return records.Where(x => x.Author == author).ToImmutableList();
  }

  public static ImmutableList<GitLogRecord> IncludeAuthor(this IEnumerable<GitLogRecord> records, Regex author)
  {
    return records.Where(x => author.IsMatch(x.Author)).ToImmutableList();
  }

  public static ImmutableList<GitLogRecord> ExcludeAuthor(this IEnumerable<GitLogRecord> records, string author)
  {
    var excluded = records.Where(x => x.Author == author);
    return records.Except(excluded).ToImmutableList();
  }

  public static ImmutableList<GitLogRecord> ExcludeAuthor(this IEnumerable<GitLogRecord> records, Regex author)
  {
    var excluded = records.Where(x => author.IsMatch(x.Author));
    return records.Except(excluded).ToImmutableList();
  }

  public static ImmutableList<GitLogRecord> RenameAuthor(this IEnumerable<GitLogRecord> records, string toAuthor, string fromAuthor)
  {
    return records.Select(rec
      => new GitLogRecord(
        Author: rec.Author == fromAuthor ? toAuthor : rec.Author,
        RepoName: rec.RepoName,
        Commit: rec.Commit,
        Date: rec.Date,
        Message: rec.Message,
        Name: rec.Name)).ToImmutableList();
  }

  public static ImmutableList<GitLogRecord> IncludeName(this IEnumerable<GitLogRecord> records, string name)
  {
    return IncludeName(records, new Regex(name));
  }

  public static ImmutableList<GitLogRecord> IncludeName(this IEnumerable<GitLogRecord> records, Regex name)
  {
    return records.Where(x => name.IsMatch(x.Name)).ToImmutableList();
  }

  public static ImmutableList<GitLogRecord> ExcludeName(this IEnumerable<GitLogRecord> records, string name)
  {
    return ExcludeName(records, new Regex(name));
  }

  public static ImmutableList<GitLogRecord> ExcludeName(this IEnumerable<GitLogRecord> records, Regex name)
  {
    var excluded = records.Where(x => name.IsMatch(x.Name));
    return records.Except(excluded).ToImmutableList();
  }

  public static ImmutableList<GitLogRecord> IncludeMessage(this IEnumerable<GitLogRecord> records, string message)
  {
    return IncludeMessage(records, new Regex(message));
  }

  public static ImmutableList<GitLogRecord> IncludeMessage(this IEnumerable<GitLogRecord> records, Regex message)
  {
    return records.Where(x => message.IsMatch(x.Message)).ToImmutableList();
  }

  public static ImmutableList<GitLogRecord> ExcludeMessage(this IEnumerable<GitLogRecord> records, string message)
  {
    return ExcludeMessage(records, new Regex(message));
  }

  public static ImmutableList<GitLogRecord> ExcludeMessage(this IEnumerable<GitLogRecord> records, Regex message)
  {
    var excluded = records.Where(x => message.IsMatch(x.Message));
    return records.Except(excluded).ToImmutableList();
  }

  public static ImmutableList<GitLogRecord> IncludeCommit(this IEnumerable<GitLogRecord> records, string commit)
  {
    return records.Where(x => x.Commit == commit).ToImmutableList();
  }

  public static ImmutableList<GitLogRecord> IncludeCommit(this IEnumerable<GitLogRecord> records, Regex commit)
  {
    return records.Where(x => commit.IsMatch(x.Commit)).ToImmutableList();
  }

  public static ImmutableList<GitLogRecord> ExcludeCommit(this IEnumerable<GitLogRecord> records, string commit)
  {
    var excluded = records.Where(x => x.Commit == commit);
    return records.Except(excluded).ToImmutableList();
  }

  public static ImmutableList<GitLogRecord> ExcludeCommit(this IEnumerable<GitLogRecord> records, Regex commit)
  {
    var excluded = records.Where(x => commit.IsMatch(x.Commit));
    return records.Except(excluded).ToImmutableList();
  }

  public static ImmutableList<GitLogRecord> WithTimeRange(this IEnumerable<GitLogRecord> records, DateTime begin, DateTime end)
  {
    return records.Where(item => begin <= item.Date && item.Date <= end).ToImmutableList();
  }

  public static ImmutableList<GitLogRecord> WithElements(this IEnumerable<GitLogRecord> records, int startPosition, int count)
  {
    return records.Skip(startPosition).Take(count).ToImmutableList();
  }

  public static string ExtractPath(string nameFromGitLog)
  {
    int index = nameFromGitLog.LastIndexOf('/');
    return nameFromGitLog.Substring(0, index - 1);
  }
}
