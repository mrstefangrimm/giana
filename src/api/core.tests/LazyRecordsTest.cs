using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable S3459 // Unassigned members should be removed
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable S2933 // Fields that are only assigned in the constructor should be "readonly"
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace Giana.Api.Core.Tests;

public class LazyRecordsTest
{
  private readonly Func<IImmutableList<TestRecord>> _nullValueFactory = null;
  private readonly IImmutableList<TestRecord> _nullRecords = null;

  private readonly ImmutableArray<TestRecord> _immutableRecords = [new TestRecord("iId", "iData")];

  [Fact]
  public void Ctor_InitWithNullFactory_ThrowsArgumentNullException()
  {
    Func<LazyRecords<TestRecord>> act = () => new LazyRecords<TestRecord>(_nullValueFactory);
    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Ctor_InitWithNullRecords_ThrowsArgumentNullException()
  {
    Func<LazyRecords<TestRecord>> act = () => new LazyRecords<TestRecord>(_nullRecords);
    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Ctor_InitWithNullLazyRecords_ThrowsArgumentNullException()
  {
    Func<LazyRecords<TestRecord>> act = () => new LazyRecords<TestRecord>((LazyRecords<TestRecord>)null);
    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Value_InitWithImmutableLazyRecords_NotReferencesSameCollection()
  {
    var lazyRecords = new LazyRecords<TestRecord>(_immutableRecords);
    lazyRecords.Value.Should().NotBeSameAs(_immutableRecords);
  }

  [Fact]
  public void Value_CalledTwice_NotReferencesSameCollection()
  {
    var lazyRecords = new LazyRecords<TestRecord>(_immutableRecords);
    var value1 = lazyRecords.Value;
    var value2 = lazyRecords.Value;

    value2.Should().BeSameAs(value1);
  }

  [Fact]
  public void Reset_ValueNeverCalled_ReturnsFalse()
  {
    var lazyRecords = new LazyRecords<TestRecord>(_immutableRecords);
    lazyRecords.Reset().Should().BeFalse();
  }

  [Fact]
  public void Reset_ValueCalled_ReturnsTrue()
  {
    var lazyRecords = new LazyRecords<TestRecord>(_immutableRecords);
    _ = lazyRecords.Value;
    lazyRecords.Reset().Should().BeTrue();
  }

  [Fact]
  public void Value_CalledTwiceWithResetInBetween_NotReferencesSameCollection()
  {
    var lazyRecords = new LazyRecords<TestRecord>(_immutableRecords);
    var value1 = lazyRecords.Value;
    lazyRecords.Reset();
    var value2 = lazyRecords.Value;

    value2.Should().BeSameAs(value1);
  }

  private record TestRecord(string Id, string Data);
}
