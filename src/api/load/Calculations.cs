using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Giana.Api.Load;

internal static class Calculations
{
  internal static IEnumerable<string> ObsoleteApplyIgnore(IEnumerable<string> paths, IEnumerable<string> ignore)
  {
    List<string> files = new();
    var regexes = ignore.Select(pattern => new Regex(pattern));

    //return paths.Where(path => regexes.Any(regex => !regex.IsMatch(path)));

    foreach (var path in paths)
    {
      bool match = regexes.Any(regex => regex.IsMatch(path));
      if (!match)
      {
        files.Add(path);
      }
    }

    return files;
  }

  internal static IEnumerable<string> ReduceFilepathToGitPath(int lenRepositoryPath, IEnumerable<string> absolutePaths)
  {
    return absolutePaths.Select(absPath => absPath.Remove(0, lenRepositoryPath + 1));
  }
}
