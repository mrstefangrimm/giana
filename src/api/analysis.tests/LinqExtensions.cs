using System.Collections.Generic;

namespace Giana.Api.Analysis.Tests;

internal static class LinqExtensions
{
  public static Source Second<Source>(this IEnumerable<Source> source)
  {
    var it = source.GetEnumerator();
    it.MoveNext();
    it.MoveNext();
    return it.Current;
  }

  public static Source Third<Source>(this IEnumerable<Source> source)
  {
    var it = source.GetEnumerator();
    it.MoveNext();
    it.MoveNext();
    it.MoveNext();
    return it.Current;
  }
}
