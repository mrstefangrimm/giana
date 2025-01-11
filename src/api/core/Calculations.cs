using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace Giana.Api.Core;

public static class Calculations
{
  public static IImmutableList<GitLogRecord> IncludeAuthor(this IImmutableList<GitLogRecord> records, string author)
  {
    return records.Where(x => x.Author == author).ToImmutableList();
  }

  public static IImmutableList<GitLogRecord> IncludeAuthor(this IImmutableList<GitLogRecord> records, Regex author)
  {
    return records.Where(x => author.IsMatch(x.Author)).ToImmutableList();
  }

  public static IImmutableList<GitLogRecord> ExcludeAuthor(this IImmutableList<GitLogRecord> records, string author)
  {
    var excluded = records.Where(x => x.Author == author);
    return records.Except(excluded).ToImmutableList();
  }

  public static IImmutableList<GitLogRecord> ExcludeAuthor(this IImmutableList<GitLogRecord> records, Regex author)
  {
    var excluded = records.Where(x => author.IsMatch(x.Author));
    return records.Except(excluded).ToImmutableList();
  }

  public static IImmutableList<GitLogRecord> RenameAuthor(this IImmutableList<GitLogRecord> records, string toAuthor, string fromAuthor)
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

  public static IImmutableList<GitLogRecord> IncludeName(this IImmutableList<GitLogRecord> records, string name)
  {
    return IncludeName(records, new Regex(name));
  }

  public static IImmutableList<GitLogRecord> IncludeName(this IImmutableList<GitLogRecord> records, Regex name)
  {
    return records.Where(x => name.IsMatch(x.Name)).ToImmutableList();
  }

  public static IImmutableList<GitLogRecord> ExcludeName(this IImmutableList<GitLogRecord> records, string name)
  {
    return ExcludeName(records, new Regex(name));
  }

  public static IImmutableList<GitLogRecord> ExcludeName(this IImmutableList<GitLogRecord> records, Regex name)
  {
    var excluded = records.Where(x => name.IsMatch(x.Name));
    return records.Except(excluded).ToImmutableList();
  }

  public static IImmutableList<GitLogRecord> IncludeMessage(this IImmutableList<GitLogRecord> records, string message)
  {
    return IncludeMessage(records, new Regex(message));
  }

  public static IImmutableList<GitLogRecord> IncludeMessage(this IImmutableList<GitLogRecord> records, Regex message)
  {
    return records.Where(x => message.IsMatch(x.Message)).ToImmutableList();
  }

  public static IImmutableList<GitLogRecord> ExcludeMessage(this IImmutableList<GitLogRecord> records, string message)
  {
    return ExcludeMessage(records, new Regex(message));
  }

  public static IImmutableList<GitLogRecord> ExcludeMessage(this IImmutableList<GitLogRecord> records, Regex message)
  {
    var excluded = records.Where(x => message.IsMatch(x.Message));
    return records.Except(excluded).ToImmutableList();
  }

  public static IImmutableList<GitLogRecord> IncludeCommit(this IImmutableList<GitLogRecord> records, string commit)
  {
    return records.Where(x => x.Commit == commit).ToImmutableList();
  }

  public static IImmutableList<GitLogRecord> IncludeCommit(this IImmutableList<GitLogRecord> records, Regex commit)
  {
    return records.Where(x => commit.IsMatch(x.Commit)).ToImmutableList();
  }

  public static IImmutableList<GitLogRecord> ExcludeCommit(this IImmutableList<GitLogRecord> records, string commit)
  {
    var excluded = records.Where(x => x.Commit == commit);
    return records.Except(excluded).ToImmutableList();
  }

  public static IImmutableList<GitLogRecord> ExcludeCommit(this IImmutableList<GitLogRecord> records, Regex commit)
  {
    var excluded = records.Where(x => commit.IsMatch(x.Commit));
    return records.Except(excluded).ToImmutableList();
  }

  public static IImmutableList<GitLogRecord> WithTimeRange(this IImmutableList<GitLogRecord> records, DateTime begin, DateTime end)
  {
    return records.Where(item => begin <= item.Date && item.Date <= end).ToImmutableList();
  }

  public static IImmutableList<GitLogRecord> WithElements(this IImmutableList<GitLogRecord> records, int startPosition, int count)
  {
    return records.Skip(startPosition).Take(count).ToImmutableList();
  }

  public static string ExtractPath(string nameFromGitLog)
  {
    int index = nameFromGitLog.LastIndexOf('/');
    return nameFromGitLog.Substring(0, index - 1);
  }
}
