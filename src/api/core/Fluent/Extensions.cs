using System.Collections.Immutable;

namespace Giana.Api.Core.Fluent;

public static class Extensions
{
  public static ITimeRangeBuilder TimeRange(this IImmutableList<GitLogRecord> records)
  {
    return new TimeRangeBuilder(records);
  }

  public static ITimeRangeBuilder TimeRange(this LazyRecords<GitLogRecord> lazyRecords)
  {
    return new TimeRangeBuilder(lazyRecords);
  }

  public static IRenameBuilder Rename(this IImmutableList<GitLogRecord> records)
  {
    return new RenameBuilder(records);
  }

  public static IRenameBuilder Rename(this LazyRecords<GitLogRecord> lazyRecords)
  {
    return new RenameBuilder(lazyRecords);
  }

  public static IIncludeBuilder Include(this IImmutableList<GitLogRecord> records)
  {
    return new IncludeBuilder(records);
  }

  public static IIncludeBuilder Include(this LazyRecords<GitLogRecord> lazyRecords)
  {
    return new IncludeBuilder(lazyRecords);
  }

  public static IExcludeBuilder Exclude(this IImmutableList<GitLogRecord> records)
  {
    return new ExcludeBuilder(records);
  }

  public static IExcludeBuilder Exclude(this LazyRecords<GitLogRecord> lazyRecords)
  {
    return new ExcludeBuilder(lazyRecords);
  }

  public static IElementsRangeBuilder Elements(this IImmutableList<GitLogRecord> records)
  {
    return new ElementsRangeBuilder(records);
  }

  public static IElementsRangeBuilder Elements(this LazyRecords<GitLogRecord> lazyRecords)
  {
    return new ElementsRangeBuilder(lazyRecords);
  }
}
