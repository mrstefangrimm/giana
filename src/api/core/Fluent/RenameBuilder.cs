using System.Collections.Immutable;

namespace Giana.Api.Core.Fluent;

public interface IRenameBuilder : IReductionBuilder
{
  IRenameBuilder Author(string to, string from);
  IRenameBuilder And(string to, string from);

  IIncludeBuilder Include();
  IExcludeBuilder Exclude();
  IElementsRangeBuilder Elements();
}

internal class RenameBuilder : ReductionBuilder, IRenameBuilder
{
  internal RenameBuilder(IImmutableList<GitLogRecord> records)
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
