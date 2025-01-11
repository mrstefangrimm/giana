using System.Collections.Immutable;

namespace Giana.Api.Core.Fluent;

public interface IElementsRangeBuilder : IReductionBuilder
{
  IReductionBuilder In(int startPosition, int count);
}

internal class ElementsRangeBuilder : ReductionBuilder, IElementsRangeBuilder
{
  internal ElementsRangeBuilder(IImmutableList<GitLogRecord> records)
  {
    _query = Reduction.CreateEmpty(records);
  }

  internal ElementsRangeBuilder(LazyRecords<GitLogRecord> records)
  {
    _query = Reduction.CreateEmpty(records);
  }

  internal ElementsRangeBuilder(Reduction query)
  {
    _query = query;
  }

  public IReductionBuilder In(int startPosition, int count)
  {
    _query.Elements.Add((startPosition, count));
    return this;
  }
}
