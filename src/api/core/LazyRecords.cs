using System;
using System.Collections.Generic;

namespace Giana.Api.Core;

public sealed class LazyRecords<RecordType>
{
  private readonly Func<IEnumerable<RecordType>> _valueFactory;

  public LazyRecords(Func<IEnumerable<RecordType>> valueFactory)
  {
    _valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
  }

  public LazyRecords(IEnumerable<RecordType> resolvedRecords)
  {
    if (resolvedRecords == null) throw new ArgumentNullException(nameof(resolvedRecords));
    _valueFactory = () => resolvedRecords;
  }

  public LazyRecords(LazyRecords<RecordType> other)
  {
    if (other == null) throw new ArgumentNullException(nameof(other));
    _valueFactory = other._valueFactory;
  }

  public IEnumerable<RecordType> Value => _valueFactory();
}
