using System;

namespace Giana.Api.Analysis;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class AnalyzerAttribute : Attribute
{
  public string Analyzer { get; private set; }

  public AnalyzerAttribute(string name)
  {
    Analyzer = name;
  }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class AnalyzerExecuteAttribute : Attribute
{
  public string[] AnalyzerExecute { get; private set; }

  public AnalyzerExecuteAttribute(string[] outputFormats)
  {
    AnalyzerExecute = outputFormats;
  }
}
