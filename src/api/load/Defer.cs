using System;

namespace Giana.Api.Load;

public sealed class Defer : IDisposable
{
  Action _deferAction;

  public Defer(Action deferAction)
  {
    _deferAction = deferAction;
  }

  public void Dispose()
  {
    var defer = _deferAction;
    _deferAction = null;

    defer?.Invoke();
  }
}
