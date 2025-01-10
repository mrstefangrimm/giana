using Giana.Api.Core;
using System.Collections.Immutable;
using System.IO;
using System.Threading;

namespace Giana.Api.Analysis;

public record ExecutionContext(
  IImmutableList<GitLogRecord> LogRecords,
  IImmutableList<string> ActiveNames,
  string OutputFormat,
  TextWriter Output,
  CancellationToken Cancellation);
