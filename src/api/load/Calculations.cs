using System.Collections.Immutable;
using System.Linq;

namespace Giana.Api.Load;

internal static class Calculations
{
  internal static IImmutableList<string> ReduceFilepathToGitPath(int lenRepositoryPath, IImmutableList<string> absolutePaths)
  {
    return absolutePaths.Select(absPath => absPath.Remove(0, lenRepositoryPath + 1)).ToImmutableList();
  }
}
