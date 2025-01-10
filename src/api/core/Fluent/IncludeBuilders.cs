using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Giana.Api.Core.Fluent;

public interface IIncludeBuilder : IReductionBuilder
{
  IIncludeNameBuilder Name(Regex name);
  IIncludeCommitBuilder Commit(string commit);
  IIncludeAuthorBuilder Author(string author);
  IIncludeMessageBuilder Message(Regex message);
}

public interface IIncludeNameBuilder : IReductionBuilder
{
  IIncludeNameBuilder And(Regex name);
  IIncludeCommitBuilder Commit(string commit);
  IIncludeAuthorBuilder Author(string author);
  IIncludeMessageBuilder Message(Regex message);

  IExcludeBuilder Exclude();
  IElementsRangeBuilder Elements();
}

public interface IIncludeCommitBuilder : IReductionBuilder
{
  IIncludeCommitBuilder And(string commit);
  IIncludeNameBuilder Name(Regex name);
  IIncludeAuthorBuilder Author(string author);
  IIncludeMessageBuilder Message(Regex message);

  IExcludeBuilder Exclude();
  IElementsRangeBuilder Elements();
}

public interface IIncludeAuthorBuilder : IReductionBuilder
{
  IIncludeAuthorBuilder And(string author);
  IIncludeNameBuilder Name(Regex name);
  IIncludeCommitBuilder Commit(string commit);
  IIncludeMessageBuilder Message(Regex message);

  IExcludeBuilder Exclude();
  IElementsRangeBuilder Elements();
}

public interface IIncludeMessageBuilder : IReductionBuilder
{
  IIncludeMessageBuilder And(Regex message);
  IIncludeNameBuilder Name(Regex name);
  IIncludeCommitBuilder Commit(string commit);
  IIncludeAuthorBuilder Author(string author);

  IExcludeBuilder Exclude();
  IElementsRangeBuilder Elements();
}

internal class IncludeBuilder : ReductionBuilder, IIncludeBuilder
{
  internal IncludeBuilder(IImmutableList<GitLogRecord> records)
  {
    _query = Reduction.CreateEmpty(records);
  }

  internal IncludeBuilder(LazyRecords<GitLogRecord> records)
  {
    _query = Reduction.CreateEmpty(records);
  }

  internal IncludeBuilder(Reduction query)
  {
    _query = query;
  }

  public IIncludeNameBuilder Name(Regex name) => new IncludeNameBuilder(_query, name);
  public IIncludeCommitBuilder Commit(string commit) => new IncludeCommitBuilder(_query, commit);
  public IIncludeAuthorBuilder Author(string author) => new IncludeAuthorBuilder(_query, author);
  public IIncludeMessageBuilder Message(Regex message) => new IncludeMessageBuilder(_query, message);
}

internal class IncludeNameBuilder : ReductionBuilder, IIncludeNameBuilder
{
  internal IncludeNameBuilder(Reduction query, Regex name)
  {
    _query = query;
    _query.IncludeNames.Add(name);
  }

  public IIncludeNameBuilder And(Regex name)
  {
    _query.IncludeNames.Add(name);
    return this;
  }

  public IIncludeCommitBuilder Commit(string commit) => new IncludeCommitBuilder(_query, commit);
  public IIncludeAuthorBuilder Author(string author) => new IncludeAuthorBuilder(_query, author);
  public IIncludeMessageBuilder Message(Regex message) => new IncludeMessageBuilder(_query, message);
}

internal class IncludeCommitBuilder : ReductionBuilder, IIncludeCommitBuilder
{
  internal IncludeCommitBuilder(Reduction query, string commit)
  {
    _query = query;
    _query.IncludeCommits.Add(commit);
  }

  public IIncludeCommitBuilder And(string commit)
  {
    _query.IncludeCommits.Add(commit);
    return this;
  }

  public IIncludeNameBuilder Name(Regex name) => new IncludeNameBuilder(_query, name);
  public IIncludeAuthorBuilder Author(string author) => new IncludeAuthorBuilder(_query, author);
  public IIncludeMessageBuilder Message(Regex message) => new IncludeMessageBuilder(_query, message);
}

internal class IncludeAuthorBuilder : ReductionBuilder, IIncludeAuthorBuilder
{
  internal IncludeAuthorBuilder(Reduction query, string author)
  {
    _query = query;
    _query.IncludeAuthors.Add(author);
  }

  public IIncludeAuthorBuilder And(string author)
  {
    _query.IncludeAuthors.Add(author);
    return this;
  }

  public IIncludeNameBuilder Name(Regex name) => new IncludeNameBuilder(_query, name);
  public IIncludeCommitBuilder Commit(string commit) => new IncludeCommitBuilder(_query, commit);
  public IIncludeMessageBuilder Message(Regex message) => new IncludeMessageBuilder(_query, message);
}

internal class IncludeMessageBuilder : ReductionBuilder, IIncludeMessageBuilder
{
  internal IncludeMessageBuilder(Reduction query, Regex message)
  {
    _query = query;
    _query.IncludeMessages.Add(message);
  }

  public IIncludeMessageBuilder And(Regex message)
  {
    _query.IncludeMessages.Add(message);
    return this;
  }

  public IIncludeNameBuilder Name(Regex name) => new IncludeNameBuilder(_query, name);
  public IIncludeCommitBuilder Commit(string commit) => new IncludeCommitBuilder(_query, commit);
  public IIncludeAuthorBuilder Author(string author) => new IncludeAuthorBuilder(_query, author);
}
