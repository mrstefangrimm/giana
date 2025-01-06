using System.Collections.Generic;

namespace Giana.Api.Core.Fluent;

public static class Extensions
{
  public static ITimeRangeBuilder TimeRange(this IEnumerable<GitLogRecord> records)
  {
    return new TimeRangeBuilder(records);
  }

  public static ITimeRangeBuilder TimeRange(this LazyRecords<GitLogRecord> lazyRecords)
  {
    return new TimeRangeBuilder(lazyRecords);
  }

  public static IRenameBuilder Rename(this IEnumerable<GitLogRecord> records)
  {
    return new RenameBuilder(records);
  }

  public static IRenameBuilder Rename(this LazyRecords<GitLogRecord> lazyRecords)
  {
    return new RenameBuilder(lazyRecords);
  }

  public static IIncludeBuilder Include(this IEnumerable<GitLogRecord> records)
  {
    return new IncludeBuilder(records);
  }

  public static IIncludeBuilder Include(this LazyRecords<GitLogRecord> lazyRecords)
  {
    return new IncludeBuilder(lazyRecords);
  }

  public static IExcludeBuilder Exclude(this IEnumerable<GitLogRecord> records)
  {
    return new ExcludeBuilder(records);
  }

  public static IExcludeBuilder Exclude(this LazyRecords<GitLogRecord> lazyRecords)
  {
    return new ExcludeBuilder(lazyRecords);
  }

  public static IElementsRangeBuilder Elements(this IEnumerable<GitLogRecord> records)
  {
    return new ElementsRangeBuilder(records);
  }

  public static IElementsRangeBuilder Elements(this LazyRecords<GitLogRecord> lazyRecords)
  {
    return new ElementsRangeBuilder(lazyRecords);
  }
}
