using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Giana.Api.Load;

internal static class Calculations
{
  internal static IImmutableList<string> ReduceFilepathToGitPath(int lenRepositoryPath, IImmutableList<string> absolutePaths)
  {
    return absolutePaths.Select(absPath => absPath.Remove(0, lenRepositoryPath + 1)).ToImmutableList();
  }

  internal static void ThrowIfCancellationRequested(this CancellationToken cancellationToken, Action cleanup)
  {
    if (cancellationToken.IsCancellationRequested)
    {
      cleanup();
      throw new OperationCanceledException(cancellationToken);
    }
  }
}
