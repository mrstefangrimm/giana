using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Giana.Api.Core;

public sealed class LazyRecords<RecordType>
{
  private readonly Func<IImmutableList<RecordType>> _valueFactory;
  private IImmutableList<RecordType> _value;

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

  public bool Reset()
  {
    bool changed = _value != null;
    _value = null;

    return changed;
  }

  public IImmutableList<RecordType> Value
  {
    get
    {
      _value = _value ?? _valueFactory();
      return _value;
    }
  }

  public Task<IImmutableList<RecordType>> ValueAsync => Task.Factory.StartNew(() => Value);
}
