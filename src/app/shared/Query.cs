using System;
using System.Collections.Generic;

namespace Giana.App.Shared;

public class Query
{
  public List<string> Sources { get; set; }
  public string Analyzer { get; set; }
  public string OutputFormat { get; set; }
  public List<TimePeriod> TimeRanges { get; set; } = [];
  public List<Author> Renames { get; set; } = [];
  public Reduction Includes { get; set; } = new Reduction();
  public Reduction Excludes { get; set; } = new Reduction();
  public DateTime? CommitsFrom { get; set; }
}
