using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace Giana.Api.Shared.Fluent;

internal record Reduction(
  LazyRecords<GitLogRecord> LazyRecords,
  ICollection<Regex> IncludeNames,
  ICollection<string> IncludeCommits,
  ICollection<string> IncludeAuthors,
  ICollection<Regex> IncludeMessages,
  ICollection<string> ExcludeCommits,
  ICollection<string> ExcludeAuthors,
  ICollection<(string To, string From)> RenameAuthors)
{
  public static Reduction CreateEmpty(IEnumerable<GitLogRecord> records)
  {
    return new Reduction(new LazyRecords<GitLogRecord>(records), new List<Regex>(), new List<string>(), new List<string>(), new List<Regex>(), new List<string>(), new List<string>(), new List<(string, string)>());
  }

  public static Reduction CreateEmpty(LazyRecords<GitLogRecord> lazyRecords)
  {
    return new Reduction(new LazyRecords<GitLogRecord>(lazyRecords), new List<Regex>(), new List<string>(), new List<string>(), new List<Regex>(), new List<string>(), new List<string>(), new List<(string, string)>());
  }
}

public interface IReductionBuilder
{
  LazyRecords<GitLogRecord> Build();
}

internal class ReductionBuilder : IReductionBuilder
{
  protected Reduction _query;

  public LazyRecords<GitLogRecord> Build()
  {
    ImmutableList<GitLogRecord> Invoke()
    {
      var includedNames = _query.LazyRecords.Value.Where(item =>
        _query.IncludeNames.Count == 0 || _query.IncludeNames.Any(regex => regex.IsMatch(item.Name)));

      var includedNamesAndCommits = includedNames.Where(item =>
        _query.IncludeCommits.Count == 0 || _query.IncludeCommits.Contains(item.Commit));

      var includedNamesAndCommitsAndAuthors = includedNamesAndCommits.Where(item =>
        _query.IncludeAuthors.Count == 0 || _query.IncludeAuthors.Contains(item.Author));

      var includedNamesAndCommitsAndAuthorsAndMessages = includedNamesAndCommitsAndAuthors.Where(item =>
        _query.IncludeMessages.Count == 0 || _query.IncludeMessages.Any(regex => regex.IsMatch(item.Message))).ToArray();

      var excluded = includedNamesAndCommitsAndAuthorsAndMessages.Where(item =>
        _query.ExcludeCommits.Contains(item.Commit) ||
        _query.ExcludeAuthors.Contains(item.Author));

      var includedAndExcluded = includedNamesAndCommitsAndAuthorsAndMessages.Except(excluded).ToImmutableList();

      foreach (var renameItem in _query.RenameAuthors)
      {
        includedAndExcluded = includedAndExcluded.Select(rec => new GitLogRecord(
          Author: rec.Author == renameItem.From ? renameItem.To : rec.Author,
          RepoName: rec.RepoName,
          Commit: rec.Commit,
          Date: rec.Date,
          Message: rec.Message,
          Name: rec.Name)).ToImmutableList();
      }

      return includedAndExcluded;
    }

    return new LazyRecords<GitLogRecord>(Invoke);
  }

  public IExcludeBuilder Exclude()
  {
    return new ExcludeBuilder(_query);
  }

  public IIncludeBuilder Include()
  {
    return new IncludeBuilder(_query);
  }

  public IRenameBuilder Rename()
  {
    return new RenameBuilder(_query);
  }
}
