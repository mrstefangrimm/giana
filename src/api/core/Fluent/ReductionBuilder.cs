using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Giana.Api.Core.Fluent;

internal record Reduction(
  LazyRecords<GitLogRecord> LazyRecords,
  ICollection<Regex> IncludeNames,
  ICollection<string> IncludeCommits,
  ICollection<string> IncludeAuthors,
  ICollection<Regex> IncludeMessages,
  ICollection<Regex> ExcludeNames,
  ICollection<string> ExcludeCommits,
  ICollection<string> ExcludeAuthors,
  ICollection<Regex> ExcludeMessages,
  ICollection<(string To, string From)> RenameAuthors,
  ICollection<(DateTime Begin, DateTime End)> TimePeriods,
  ICollection<(int StartPosition, int Count)> Elements)
{
  public static Reduction CreateEmpty(IImmutableList<GitLogRecord> records)
  {
    return new Reduction(new LazyRecords<GitLogRecord>(records), new List<Regex>(), new List<string>(), new List<string>(), new List<Regex>(), new List<Regex>(), new List<string>(), new List<string>(), new List<Regex>(), new List<(string, string)>(), new List<(DateTime, DateTime)>(), new List<(int, int)>());
  }

  public static Reduction CreateEmpty(LazyRecords<GitLogRecord> lazyRecords)
  {
    return new Reduction(new LazyRecords<GitLogRecord>(lazyRecords), new List<Regex>(), new List<string>(), new List<string>(), new List<Regex>(), new List<Regex>(), new List<string>(), new List<string>(), new List<Regex>(), new List<(string, string)>(), new List<(DateTime, DateTime)>(), new List<(int, int)>());
  }
}

public interface IReductionBuilder
{
  IImmutableList<GitLogRecord> Build();
  Task<IImmutableList<GitLogRecord>> BuildAsync();
  LazyRecords<GitLogRecord> BuildLazy();
}

internal class ReductionBuilder : IReductionBuilder
{
  protected Reduction _query;

  public IImmutableList<GitLogRecord> Build()
  {
    ImmutableList<GitLogRecord> reducedList = _query.LazyRecords.Value
      .Where(item => _query.TimePeriods.Count == 0 || _query.TimePeriods.Any(tp => tp.Begin <= item.Date && item.Date <= tp.End)).ToImmutableList();

    foreach (var renameItem in _query.RenameAuthors)
    {
      reducedList = reducedList.Select(rec => new GitLogRecord(
        Author: rec.Author == renameItem.From ? renameItem.To : rec.Author,
        RepoName: rec.RepoName,
        Commit: rec.Commit,
        Date: rec.Date,
        Message: rec.Message,
        Name: rec.Name)).ToImmutableList();
    }

    var includedNames = reducedList.Where(item =>
      _query.IncludeNames.Count == 0 || _query.IncludeNames.Any(regex => regex.IsMatch(item.Name)));

    var includedNamesAndCommits = includedNames.Where(item =>
      _query.IncludeCommits.Count == 0 || _query.IncludeCommits.Contains(item.Commit));

    var includedNamesAndCommitsAndAuthors = includedNamesAndCommits.Where(item =>
      _query.IncludeAuthors.Count == 0 || _query.IncludeAuthors.Contains(item.Author));

    var includedNamesAndCommitsAndAuthorsAndMessages = includedNamesAndCommitsAndAuthors.Where(item =>
      _query.IncludeMessages.Count == 0 || _query.IncludeMessages.Any(regex => regex.IsMatch(item.Message))).ToArray();

    var excluded = includedNamesAndCommitsAndAuthorsAndMessages.Where(item =>
    _query.ExcludeNames.Any(regex => regex.IsMatch(item.Name)) ||
      _query.ExcludeCommits.Contains(item.Commit) ||
      _query.ExcludeAuthors.Contains(item.Author) ||
      _query.ExcludeMessages.Any(regex => regex.IsMatch(item.Message)));

    var includedAndExcluded = includedNamesAndCommitsAndAuthorsAndMessages.Except(excluded).ToImmutableList();

    if (_query.Elements.Any())
    {
      return includedAndExcluded.Skip(_query.Elements.First().StartPosition).Take(_query.Elements.First().Count).ToImmutableList();
    }

    return includedAndExcluded;
  }

  public Task<IImmutableList<GitLogRecord>> BuildAsync()
  {
    return Task.Factory.StartNew(Build);
  }

  public LazyRecords<GitLogRecord> BuildLazy()
  {
    return new LazyRecords<GitLogRecord>(Build);
  }

  public IRenameBuilder Rename()
  {
    return new RenameBuilder(_query);
  }

  public IExcludeBuilder Exclude()
  {
    return new ExcludeBuilder(_query);
  }

  public IIncludeBuilder Include()
  {
    return new IncludeBuilder(_query);
  }

  public IElementsRangeBuilder Elements()
  {
    return new ElementsRangeBuilder(_query);
  }
}
