using Giana.Api.Analysis;
using Giana.Api.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;

namespace Giana.App.Shared;

public class QueryRoutine
{
  public string OutputFormat { get; set; }
  public TextWriter OutputWriter { get; set; }

  public ICollection<string> Sources { get; set; }
  public DateTime Deadline { get; set; }

  public ICollection<(Func<IEnumerable<GitLogRecord>, DateTime, DateTime, ImmutableList<GitLogRecord>> Invoke, DateTime Begin, DateTime End)> TimeRanges { get; set; }
  public ICollection<(Func<IEnumerable<GitLogRecord>, string, string, ImmutableList<GitLogRecord>> Invoke, string To, string From)> Renames { get; set; }
  public ICollection<(Func<IEnumerable<GitLogRecord>, Regex, ImmutableList<GitLogRecord>> Invoke, Regex Argument)> Reductions { get; set; }

  public Action<ExecutionContext> Analyze { get; set; }
}
