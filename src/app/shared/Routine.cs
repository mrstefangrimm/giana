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
  public string OutputFormat { get; internal set; }

  public ICollection<string> Sources { get; internal set; }
  public DateTime? CommitsFrom { get; internal set; }

  public ICollection<(Func<IImmutableList<GitLogRecord>, DateTime, DateTime, IImmutableList<GitLogRecord>> Invoke, DateTime Begin, DateTime End)> TimeRanges { get; internal set; }
  public ICollection<(Func<IImmutableList<GitLogRecord>, string, string, IImmutableList<GitLogRecord>> Invoke, string To, string From)> Renames { get; internal set; }
  public ICollection<(Func<IImmutableList<GitLogRecord>, Regex, IImmutableList<GitLogRecord>> Invoke, Regex Argument)> Reductions { get; internal set; }

  public Action<ExecutionContext> Analyze { get; internal set; }
}
