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

  private readonly IEnumerable<TestRecord> _sharedRecords = new[] { new TestRecord("sId", "sData") };
  private readonly ImmutableArray<TestRecord> _immutableRecords = ImmutableArray.Create(new TestRecord("iId", "iData"));

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


  //[Fact]
  //public void Value_InitWithSharedLazyRecords_ReferenceSameCollection()
  //{
  //  var lazyRecords = new LazyRecords<TestRecord>(_sharedRecords);
  //  lazyRecords.Value.Should().BeSameAs(_sharedRecords);
  //}

  [Fact]
  public void Value_InitWithImmutableLazyRecords_NotReferenceSameCollection()
  {
    var lazyRecords = new LazyRecords<TestRecord>(_immutableRecords);
    lazyRecords.Value.Should().NotBeSameAs(_immutableRecords);
  }

  private record TestRecord(string Id, string Data);
}
