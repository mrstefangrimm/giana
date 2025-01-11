using FluentAssertions;
using System;

namespace Giana.Api.Load.Tests;

public class DeferTest
{
  [Fact]
  public void Ctor_InitWithNull_DisposeNotThrowingException()
  {
    var defer = new Defer(null);

    Action dispose = defer.Dispose;

    dispose.Should().NotThrow();
  }

  [Fact]
  public void Ctor_InitWithAction_DisposeActionIsCalledOnce()
  {
    int disposeCounter = 0;

    Action defering = () => { disposeCounter++; };

    var defer = new Defer(defering);

    defer.Dispose();

    disposeCounter.Should().Be(1);
  }

  [Fact]
  public void Ctor_InitWithAction_DisposedTwiceActionIsCalledOnce()
  {
    int disposeCounter = 0;

    Action defering = () => { disposeCounter++; };

    var defer = new Defer(defering);

    defer.Dispose();
    defer.Dispose();

    disposeCounter.Should().Be(1);
  }
}
