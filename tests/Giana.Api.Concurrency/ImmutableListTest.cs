using Giana.Api.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Giana.Api.Concurrency;

/// <summary>
/// "lock" is required if `AddRange` is called.
/// </summary>
public class ImmutableListTest
{
  private readonly Random _random = new Random();
  private static readonly object _lock = new object();

  [Fact]
  public void AddRange_CalledFromDifferentTasks_ItemsAreNotMixed()
  {
    var mergedRecords = ImmutableList.Create<GitLogRecord>();

    var tasks = new Task<IImmutableList<GitLogRecord>>[100];
    for (int i = 0; i < tasks.Length; i++)
    {
      var task = new Task<IImmutableList<GitLogRecord>>(() =>
      {
        return CreateItems($"task{i}");
      });
      task.GetAwaiter().OnCompleted(() =>
      {
        lock (_lock)
        {
          mergedRecords = mergedRecords.AddRange(task.Result);
        }
      });
      tasks[i] = task;
    }

    for (int i = 0; i < tasks.Length; i++)
    {
      tasks[i].Start();
    }

    Task.WaitAll(tasks);

    // Wait for the OnCompleted event handlers
    Thread.Sleep(1000);

    Assert.Equal(tasks.Length * 10, mergedRecords.Count);

    for (int i = 0; i < tasks.Length * 10; i++)
    {
      Assert.Equal(i % 10, int.Parse(mergedRecords[i].Commit));
    }
  }

  private IImmutableList<GitLogRecord> CreateItems(string taskName)
  {
    var items = new List<GitLogRecord>();
    for (int i = 0; i < 10; i++)
    {
      items.Add(new GitLogRecord(taskName, string.Empty, $"{i}", string.Empty, string.Empty, DateTime.Now));
    }
    Thread.Sleep(_random.Next(20));

    return items.ToImmutableList();
  }
}
