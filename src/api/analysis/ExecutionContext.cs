using Giana.Api.Core;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Giana.Api.Analysis;

public record ExecutionContext(
  IEnumerable<GitLogRecord> LogRecords,
  IEnumerable<string> ActiveNames,
  string OutputFormat,
  StreamWriter Output,
  CancellationToken Cancellation);
