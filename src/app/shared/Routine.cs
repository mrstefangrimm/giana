using Giana.Api.Analysis;
using Giana.Api.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;

namespace Giana.App.Shared;

public class Routine
{
  public string OutputFormat { get; set; }
  public TextWriter OutputWriter { get; set; }

  public ICollection<string> Sources { get; set; }
  public DateTime? CommitsFrom { get; set; }

  public ICollection<(Func<IImmutableList<GitLogRecord>, DateTime, DateTime, IImmutableList<GitLogRecord>> Invoke, DateTime Begin, DateTime End)> TimeRanges { get; set; }
  public ICollection<(Func<IImmutableList<GitLogRecord>, string, string, IImmutableList<GitLogRecord>> Invoke, string To, string From)> Renames { get; set; }
  public ICollection<(Func<IImmutableList<GitLogRecord>, Regex, IImmutableList<GitLogRecord>> Invoke, Regex Argument)> Reductions { get; set; }

  public Action<ExecutionContext> Analyze { get; set; }
}
