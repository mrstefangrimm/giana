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
    ArgumentNullException.ThrowIfNull(resolvedRecords);
    _valueFactory = () => resolvedRecords;
  }

  public LazyRecords(LazyRecords<RecordType> other)
  {
    ArgumentNullException.ThrowIfNull(other);
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
