using System;
using System.Collections.Generic;

namespace Giana.Api.Shared.Fluent;

public interface ITimeRangeBuilder : IReductionBuilder
{
  ITimeRangeBuilder TimePeriod(DateTime begin, DateTime end);
  ITimeRangeBuilder And(DateTime begin, DateTime end);

  IRenameBuilder Rename();
  IIncludeBuilder Include();
  IExcludeBuilder Exclude();
}

internal class TimeRangeBuilder : ReductionBuilder, ITimeRangeBuilder
{
  internal TimeRangeBuilder(IEnumerable<GitLogRecord> records)
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

  public ITimeRangeBuilder TimePeriod(DateTime begin, DateTime end)
  {
    _query.TimePeriods.Add((begin, end));
    return this;
  }

  public ITimeRangeBuilder And(DateTime begin, DateTime end)
  {
    return TimePeriod(begin, end);
  }
}
