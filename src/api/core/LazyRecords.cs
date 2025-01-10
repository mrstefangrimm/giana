using System;
using System.Collections.Immutable;

namespace Giana.Api.Core;

public sealed class LazyRecords<RecordType>
{
  private readonly Func<IImmutableList<RecordType>> _valueFactory;

  public LazyRecords(Func<IImmutableList<RecordType>> valueFactory)
  {
    _valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
  }

  public LazyRecords(IImmutableList<RecordType> resolvedRecords)
  {
    if (resolvedRecords == null) throw new ArgumentNullException(nameof(resolvedRecords));
    _valueFactory = () => resolvedRecords;
  }

  public LazyRecords(LazyRecords<RecordType> other)
  {
    if (other == null) throw new ArgumentNullException(nameof(other));
    _valueFactory = other._valueFactory;
  }

  public IImmutableList<RecordType> Value => _valueFactory();
}
