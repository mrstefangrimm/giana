using System;
using System.Collections.Immutable;

namespace Giana.Api.Core.Fluent;

public interface ITimeRangeBuilder : IReductionBuilder
{
  ITimeRangeBuilder In(DateTime begin, DateTime end);
  ITimeRangeBuilder And(DateTime begin, DateTime end);

  IRenameBuilder Rename();
  IIncludeBuilder Include();
  IExcludeBuilder Exclude();
  IElementsRangeBuilder Elements();
}

internal class TimeRangeBuilder : ReductionBuilder, ITimeRangeBuilder
{
  internal TimeRangeBuilder(IImmutableList<GitLogRecord> records)
  {
    _query = Reduction.CreateEmpty(records);
  }

  internal TimeRangeBuilder(LazyRecords<GitLogRecord> records)
  {
    _query = Reduction.CreateEmpty(records);
  }

  internal TimeRangeBuilder(Reduction query)
  {
    _query = query;
  }

  public ITimeRangeBuilder In(DateTime begin, DateTime end)
  {
    _query.TimePeriods.Add((begin, end));
    return this;
  }

  public ITimeRangeBuilder And(DateTime begin, DateTime end)
  {
    return In(begin, end);
  }
}
