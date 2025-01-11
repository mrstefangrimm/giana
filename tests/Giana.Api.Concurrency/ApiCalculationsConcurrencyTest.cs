using Giana.Api.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Giana.Api.Concurrency;

public class ApiCalculationsConcurrencyTest
{
  static long counter = 0;

  [Fact]
  public void ExcludeAuthor_RunFor100sec_Successful()
  {
    var mutableData = new ConcurrentBag<GitLogRecord>();
    var excludeRegex = new Regex("J.e D.e.*");

    var writer = StartWriterTask(mutableData);
    var reader = StartReaderTask(() => Calculations.ExcludeAuthor(mutableData.ToImmutableList(), excludeRegex));

    WaitAndAssert(writer, reader);
  }

  [Fact]
  public void ExcludeName_RunFor100sec_Successful()
  {
    var mutableData = new ConcurrentBag<GitLogRecord>();
    var excludeRegex = new Regex("[C|c]ore/.acka.e/Fi.*.cs");

    var writer = StartWriterTask(mutableData);
    var reader = StartReaderTask(() => Calculations.ExcludeName(mutableData.ToImmutableList(), excludeRegex));

    WaitAndAssert(writer, reader);
  }

    [Fact]
  public void ExcludeMessage_RunFor100sec_Successful()
  {
    var mutableData = new ConcurrentBag<GitLogRecord>();
    var excludeRegex = new Regex("[M|m]ess.*wi.*no.*");

    var writer = StartWriterTask(mutableData);
    var reader = StartReaderTask(() => Calculations.ExcludeMessage(mutableData.ToImmutableList(), excludeRegex));

    WaitAndAssert(writer, reader);
  }

  private static Task StartWriterTask(ConcurrentBag<GitLogRecord> mutableData)
  {
    // fill with some data
    for (int i = 0; i < 10; i++)
    {
      mutableData.Add(CreateDummyRecord());
    }

    return Task.Factory.StartNew(() =>
    {
      // Run for 100 seconds (10000 * 10 / 1000 = 10 * 10 = 100)
      for (int i = 0; i < 10000; i++)
      {
        mutableData.Add(CreateDummyRecord());
        Thread.Sleep(10);
      }
    });
  }

  private static Task StartReaderTask(Func<IImmutableList<GitLogRecord>> processing)
  {
    return Task.Factory.StartNew(() =>
    {
      while (true)
      {
        var immutableResult = processing();
        if (immutableResult.Any())
        {
          throw new InvalidOperationException(string.Join(',', immutableResult.Select(x => x.Commit)));
        }
        Thread.Sleep(1000);
      }
    });
  }

  private static void WaitAndAssert(Task writer, Task reader)
  {
    Task.WaitAny(writer, reader);

    Assert.Null(reader.Exception?.Message);
    Assert.Equal(TaskStatus.Running, reader.Status);
    Assert.Equal(TaskStatus.RanToCompletion, writer.Status);
  }

  private static GitLogRecord CreateDummyRecord()
  {
    counter++;
    return new GitLogRecord(string.Empty, $"Core/Package/File{counter}.cs", $"Commit{counter}", $"Joe Doe{counter}", $"Message with no {counter}", DateTime.Now);
  }
}