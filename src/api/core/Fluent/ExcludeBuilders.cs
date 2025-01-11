using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Giana.Api.Core.Fluent;

public interface IExcludeBuilder : IReductionBuilder
{
  IExcludeNameBuilder Name(Regex name);
  IExcludeCommitBuilder Commit(string commit);
  IExcludeAuthorBuilder Author(string author);
  IExcludeMessageBuilder Message(Regex message);
}

public interface IExcludeNameBuilder : IReductionBuilder
{
  IExcludeNameBuilder And(Regex name);
  IExcludeCommitBuilder Commit(string commit);
  IExcludeAuthorBuilder Author(string author);
  IExcludeMessageBuilder Message(Regex message);

  IIncludeBuilder Include();
  IElementsRangeBuilder Elements();
}

public interface IExcludeCommitBuilder : IReductionBuilder
{
  IExcludeCommitBuilder And(string commit);
  IExcludeNameBuilder Name(Regex name);
  IExcludeAuthorBuilder Author(string author);
  IExcludeMessageBuilder Message(Regex message);

  IIncludeBuilder Include();
  IElementsRangeBuilder Elements();
}

public interface IExcludeAuthorBuilder : IReductionBuilder
{
  IExcludeAuthorBuilder And(string author);
  IExcludeNameBuilder Name(Regex name);
  IExcludeCommitBuilder Commit(string commit);
  IExcludeMessageBuilder Message(Regex message);

  IIncludeBuilder Include();
  IElementsRangeBuilder Elements();
}

public interface IExcludeMessageBuilder : IReductionBuilder
{
  IExcludeMessageBuilder And(Regex message);
  IExcludeNameBuilder Name(Regex name);
  IExcludeCommitBuilder Commit(string commit);
  IExcludeAuthorBuilder Author(string author);

  IIncludeBuilder Include();
  IElementsRangeBuilder Elements();
}

internal class ExcludeBuilder : ReductionBuilder, IExcludeBuilder
{
  internal ExcludeBuilder(IImmutableList<GitLogRecord> records)
  {
    _query = Reduction.CreateEmpty(records);
  }

  internal ExcludeBuilder(LazyRecords<GitLogRecord> records)
  {
    _query = Reduction.CreateEmpty(records);
  }

  internal ExcludeBuilder(Reduction query)
  {
    _query = query;
  }

  public IExcludeNameBuilder Name(Regex name)
  {
    return new ExcludeNameBuilder(_query, name);
  }

  public IExcludeCommitBuilder Commit(string commit)
  {
    return new ExcludeCommitBuilder(_query, commit);
  }

  public IExcludeAuthorBuilder Author(string author)
  {
    return new ExcludeAuthorBuilder(_query, author);
  }

  public IExcludeMessageBuilder Message(Regex message)
  {
    return new ExcludeMessageBuilder(_query, message);
  }
}

internal class ExcludeNameBuilder : ReductionBuilder, IExcludeNameBuilder
{
  internal ExcludeNameBuilder(Reduction query, Regex name)
  {
    _query = query;
    _query.ExcludeNames.Add(name);
  }

  public IExcludeNameBuilder And(Regex name)
  {
    _query.ExcludeNames.Add(name);
    return this;
  }

  public IExcludeNameBuilder Name(Regex name) => new ExcludeNameBuilder(_query, name);
  public IExcludeCommitBuilder Commit(string commit) => new ExcludeCommitBuilder(_query, commit);
  public IExcludeAuthorBuilder Author(string author) => new ExcludeAuthorBuilder(_query, author);
  public IExcludeMessageBuilder Message(Regex message) => new ExcludeMessageBuilder(_query, message);
}

internal class ExcludeCommitBuilder : ReductionBuilder, IExcludeCommitBuilder
{
  internal ExcludeCommitBuilder(Reduction query, string commit)
  {
    _query = query;
    _query.ExcludeCommits.Add(commit);
  }

  public IExcludeCommitBuilder And(string commit)
  {
    _query.ExcludeCommits.Add(commit);
    return this;
  }

  public IExcludeNameBuilder Name(Regex name)
  {
    return new ExcludeNameBuilder(_query, name);
  }

  public IExcludeAuthorBuilder Author(string author)
  {
    return new ExcludeAuthorBuilder(_query, author);
  }

  public IExcludeMessageBuilder Message(Regex message)
  {
    return new ExcludeMessageBuilder(_query, message);
  }
}

internal class ExcludeAuthorBuilder : ReductionBuilder, IExcludeAuthorBuilder
{
  internal ExcludeAuthorBuilder(Reduction query, string author)
  {
    _query = query;
    _query.ExcludeAuthors.Add(author);
  }

  public IExcludeAuthorBuilder And(string author)
  {
    _query.ExcludeAuthors.Add(author);
    return this;
  }

  public IExcludeNameBuilder Name(Regex name)
  {
    return new ExcludeNameBuilder(_query, name);
  }

  public IExcludeCommitBuilder Commit(string commit)
  {
    return new ExcludeCommitBuilder(_query, commit);
  }

  public IExcludeMessageBuilder Message(Regex message)
  {
    return new ExcludeMessageBuilder(_query, message);
  }
}

internal class ExcludeMessageBuilder : ReductionBuilder, IExcludeMessageBuilder
{
  internal ExcludeMessageBuilder(Reduction query, Regex message)
  {
    _query = query;
    _query.ExcludeMessages.Add(message);
  }

  public IExcludeMessageBuilder And(Regex message)
  {
    _query.ExcludeMessages.Add(message);
    return this;
  }

  public IExcludeNameBuilder Name(Regex name) => new ExcludeNameBuilder(_query, name);
  public IExcludeCommitBuilder Commit(string commit) => new ExcludeCommitBuilder(_query, commit);
  public IExcludeAuthorBuilder Author(string author) => new ExcludeAuthorBuilder(_query, author);
}
