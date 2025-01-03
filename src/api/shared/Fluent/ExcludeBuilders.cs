using System.Collections.Generic;

namespace Giana.Api.Shared.Fluent;

public interface IExcludeBuilder : IReductionBuilder
{
  IExcludeCommitBuilder Commit(string commit);
  IExcludeAuthorBuilder Author(string author);
}

public interface IExcludeCommitBuilder : IReductionBuilder
{
  IExcludeCommitBuilder And(string commit);
  IExcludeAuthorBuilder Author(string commit);

  IIncludeBuilder Include();
  IRenameBuilder Rename();
}

public interface IExcludeAuthorBuilder : IReductionBuilder
{
  IExcludeAuthorBuilder And(string author);
  IExcludeCommitBuilder Commit(string commit);

  IIncludeBuilder Include();
  IRenameBuilder Rename();
}

internal class ExcludeBuilder : ReductionBuilder, IExcludeBuilder
{
  internal ExcludeBuilder(IEnumerable<GitLogRecord> records)
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

  public IExcludeCommitBuilder Commit(string commit)
  {
    return new ExcludeCommitBuilder(_query, commit);
  }

  public IExcludeAuthorBuilder Author(string author)
  {
    return new ExcludeAuthorBuilder(_query, author);
  }
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

  public IExcludeAuthorBuilder Author(string author)
  {
    return new ExcludeAuthorBuilder(_query, author);
  }
}

internal class ExcludeAuthorBuilder : ReductionBuilder, IExcludeAuthorBuilder
{
  internal ExcludeAuthorBuilder(Reduction query, string author)
  {
    _query = query;
    _query.ExcludeAuthors.Add(author);
  }

  public IExcludeCommitBuilder Commit(string commit)
  {
    return new ExcludeCommitBuilder(_query, commit);
  }

  public IExcludeAuthorBuilder And(string author)
  {
    _query.ExcludeAuthors.Add(author);
    return this;
  }
}
