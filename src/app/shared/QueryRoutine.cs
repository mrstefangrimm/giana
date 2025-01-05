using Giana.Api.Analysis;
using Giana.Api.Shared;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;

namespace Giana.App.Shared;

public class QueryRoutine
{
  public string OutputFormat { get; set; }
  public StreamWriter OutputWriter { get; set; }

  public ICollection<string> Sources { get; set; }

  public ICollection<(Func<IEnumerable<GitLogRecord>, Regex, ImmutableList<GitLogRecord>> Invoke, Regex Argument)> Reductions { get; set; }
  public ICollection<(Func<IEnumerable<GitLogRecord>, string, string, ImmutableList<GitLogRecord>> Invoke, string To, string From)> Renames { get; set; }

  public Action<ExecutionContext> Analyze { get; set; }
}
