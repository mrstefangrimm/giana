using System;

namespace Giana.Api.Shared;

public record GitLogRecord(string RepoName, string Name, string Commit, string Author, string Message, DateTime Date)
{
  public override string ToString()
  {
    return $"{System.IO.Path.GetFileName(Name)} {Commit} {Author}";
  }
}
