using System.Collections.Generic;

namespace Giana.Api.Shared.Fluent;

public interface IRenameBuilder : IReductionBuilder
{
  IRenameBuilder Author(string to, string from);
  IRenameBuilder And(string to, string from);

  IIncludeBuilder Include();
  IExcludeBuilder Exclude();
}

internal class RenameBuilder : ReductionBuilder, IRenameBuilder
{
  internal RenameBuilder(IEnumerable<GitLogRecord> records)
  {
    _query = Reduction.CreateEmpty(records);
  }

  internal RenameBuilder(LazyRecords<GitLogRecord> records)
  {
    _query = Reduction.CreateEmpty(records);
  }

  internal RenameBuilder(Reduction query)
  {
    _query = query;
  }

  public IRenameBuilder Author(string to, string from)
  {
    _query.RenameAuthors.Add((to, from));
    return this;
  }

  public IRenameBuilder And(string to, string from)
  {
    return Author(to, from);
  }
}
