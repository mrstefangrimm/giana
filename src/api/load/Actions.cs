﻿using Giana.Api.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Giana.Api.Load;

public static class Actions
{
  public static IImmutableList<GitLogRecord> RequestGitLog(string repositoryRoot, string repositoryName, string gitExePath, DateTime? commitsFrom = null)
  {
    return GitLog(repositoryRoot, repositoryName, gitExePath, CancellationToken.None, commitsFrom);
  }

  public static async Task<IImmutableList<GitLogRecord>> RequestGitLogAsync(string repositoryRoot, string repositoryName, string gitExePath, DateTime? commitsFrom = null)
  {
    return await Task.Run(() => GitLog(repositoryRoot, repositoryName, gitExePath, CancellationToken.None, commitsFrom));
  }

  public static async Task<IImmutableList<GitLogRecord>> RequestGitLogAsync(string repositoryRoot, string repositoryName, string gitExePath, CancellationToken cancellationToken, DateTime? commitsFrom = null)
  {
    return await Task.Run(() => GitLog(repositoryRoot, repositoryName, gitExePath, cancellationToken, commitsFrom), cancellationToken);
  }

  private static IImmutableList<GitLogRecord> GitLog(string repositoryRoot, string repositoryName, string gitExePath, CancellationToken cancellationToken, DateTime? commitsFrom)
  {
    const string GitLogCmd = "log --pretty=format:\"%h^%an^%as^%s\" --name-only";

    (Process gitProcess, Action defering) = CreateAndStartGitProcess(repositoryRoot, gitExePath, GitLogCmd);
    using var defer = new Defer(defering);
    CheckStdErrOutput(gitProcess);

    var records = new List<GitLogRecord>();

    while (!gitProcess.StandardOutput.EndOfStream)
    {
      // Read commit line
      var commitLine = gitProcess.StandardOutput.ReadLine();

      var elements = commitLine.Split("^");

      if (elements.Length > 4)
      {
        string[] msgElements = new string[elements.Length - 3];
        Array.Copy(elements, 3, msgElements, 0, msgElements.Length);
        elements[3] = string.Join("^", msgElements);
      }

      // Read lines of changed files of the commit.
      var fileLine = gitProcess.StandardOutput.ReadLine();
      do
      {
        GitLogRecord change = new(
          RepoName: repositoryName,
          Name: fileLine,
          Commit: elements[0],
          Author: elements[1],
          Message: elements[3],
          Date: DateTime.Parse(elements[2]));

        records.Add(change);

        cancellationToken.ThrowIfCancellationRequested(CloseOutputStreams(gitProcess));

        fileLine = gitProcess.StandardOutput.ReadLine();

      } while (!string.IsNullOrEmpty(fileLine));

      if (commitsFrom.HasValue && records.Last().Date < commitsFrom.Value)
      {
        CloseOutputStreams(gitProcess)();
        break;
      }
    }

    return records.ToImmutableList();
  }

  public static IImmutableList<string> RequestActiveNamesFromMainBranch(string repositoryRoot, string gitExePath)
  {
    return ActiveNamesFromMainBranch(repositoryRoot, gitExePath, CancellationToken.None);
  }

  public static async Task<IImmutableList<string>> RequestActiveNamesFromMainBranchAsync(string repositoryRoot, string gitExePath)
  {
    return await Task.Run(() => ActiveNamesFromMainBranch(repositoryRoot, gitExePath, CancellationToken.None));
  }

  public static async Task<IImmutableList<string>> RequestActiveNamesFromMainBranchAsync(string repositoryRoot, string gitExePath, CancellationToken cancellationToken)
  {
    return await Task.Run(() => ActiveNamesFromMainBranch(repositoryRoot, gitExePath, cancellationToken));
  }

  private static IImmutableList<string> ActiveNamesFromMainBranch(string repositoryRoot, string gitExePath, CancellationToken cancellationToken)
  {
    string mainBranch = RequestMainBranchName(repositoryRoot, gitExePath, cancellationToken);

    string gitCmd = $"ls-tree -r {mainBranch} --name-only";

    (Process gitProcess, Action defering) = CreateAndStartGitProcess(repositoryRoot, gitExePath, gitCmd);
    using var defer = new Defer(defering);
    CheckStdErrOutput(gitProcess);

    var records = new List<string>();

    while (!gitProcess.StandardOutput.EndOfStream)
    {
      cancellationToken.ThrowIfCancellationRequested(CloseOutputStreams(gitProcess));

      var commitLine = gitProcess.StandardOutput.ReadLine();
      records.Add(commitLine);
    }

    return records.ToImmutableList();
  }

  private static string RequestMainBranchName(string repositoryRoot, string gitExePath, CancellationToken cancellationToken)
  {
    const string GitBranchCmd = "branch";

    (Process gitProcess, Action defering) = CreateAndStartGitProcess(repositoryRoot, gitExePath, GitBranchCmd);
    using var defer = new Defer(defering);
    CheckStdErrOutput(gitProcess);

    while (!gitProcess.StandardOutput.EndOfStream)
    {
      cancellationToken.ThrowIfCancellationRequested(CloseOutputStreams(gitProcess));

      var line = gitProcess.StandardOutput.ReadLine();

      if (line.StartsWith("* "))
      {
        return line.Remove(0, 2);
      }
    }

    throw new InvalidOperationException("repository main branch not found with `git branch`");
  }

  public static string RequestRepositoryName(string repositoryRoot, string gitExePath)
  {
    return RepositoryName(repositoryRoot, gitExePath, CancellationToken.None);
  }

  public static async Task<string> RequestRepositoryNameAsync(string repositoryRoot, string gitExePath)
  {
    return await Task.Run(() => RepositoryName(repositoryRoot, gitExePath, CancellationToken.None));
  }

  public static async Task<string> RequestRepositoryNameAsync(string repositoryRoot, string gitExePath, CancellationToken cancellationToken)
  {
    return await Task.Run(() => RepositoryName(repositoryRoot, gitExePath, cancellationToken));
  }

  private static string RepositoryName(string repositoryRoot, string gitExePath, CancellationToken cancellationToken)
  {
    const string GitRemoteCmd = "remote -v";

    (Process gitProcess, Action defering) = CreateAndStartGitProcess(repositoryRoot, gitExePath, GitRemoteCmd);
    using var defer = new Defer(defering);
    CheckStdErrOutput(gitProcess);

    while (!gitProcess.StandardOutput.EndOfStream)
    {
      cancellationToken.ThrowIfCancellationRequested(CloseOutputStreams(gitProcess));

      var line = gitProcess.StandardOutput.ReadLine();

      // Supported formats:
      // https://tfs-app.company.com/A/B/_git/{RepoName}
      // https://github.com/mrstefangrimm/giana.git (fetch) [blob:none]
      var split1 = line.Split("git/");
      if (split1.Length == 2)
      {
        var split2 = split1[1].Split(" ");
        return split2[0];
      }

      split1 = line.Split("/");
      if (split1.Length > 2)
      {
        var split2 = split1[split1.Length - 1].Split(".git");
        return split2[0];
      }
    }

    throw new InvalidOperationException("repository name not found with `git remote`");
  }

  public static string CreateCloneFromUri(string uri, string gitExePath)
  {
    return CloneFromUri(uri, gitExePath);
  }

  public static async Task<string> CreateCloneFromUriAsync(string uri, string gitExePath)
  {
    return await Task.Run(() => CloneFromUri(uri, gitExePath));
  }

  private static string CloneFromUri(string uri, string gitExePath)
  {
    var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    Directory.CreateDirectory(tempPath);

    string gitCloneCmd = $"clone --filter=blob:none --no-checkout --single-branch {uri} {tempPath}";

    var cloneProcess = CreateAndStartGitProcess(tempPath, gitExePath, gitCloneCmd);
    // Clone writes to the stderr, CheckStdErrOutput is therefore not called.

    using var defer = new Defer(cloneProcess.Defering);

    return tempPath;
  }

  private static (Process Git, Action Defering) CreateAndStartGitProcess(string woringDir, string gitExePath, string gitCmd)
  {
    var gitProcess = new Process();

    gitProcess.StartInfo = new ProcessStartInfo();
    gitProcess.StartInfo.FileName = gitExePath;
    gitProcess.StartInfo.Arguments = gitCmd;
    gitProcess.StartInfo.WorkingDirectory = woringDir;
    gitProcess.StartInfo.CreateNoWindow = true;
    gitProcess.StartInfo.RedirectStandardError = true;
    gitProcess.StartInfo.RedirectStandardOutput = true;
    gitProcess.StartInfo.StandardOutputEncoding = Encoding.UTF8;

    var defer = () =>
    {
      gitProcess.WaitForExit();
      gitProcess.Close();
    };

    gitProcess.Start();

    return (gitProcess, defer);
  }

  private static void CheckStdErrOutput(Process gitProcess)
  {
    // Read error. Async is used because ReadToEnd() blocks when no error.
    var stderrTask = gitProcess.StandardError.ReadToEndAsync();
    bool errorWithin1sec = stderrTask.Wait(1000);
    var err = errorWithin1sec ? stderrTask.Result : string.Empty;
    if (!string.IsNullOrEmpty(err))
    {
      throw new InvalidOperationException(err);
    }
  }

  private static Action CloseOutputStreams(Process gitProcess)
  {
    return () =>
    {
      gitProcess.StandardOutput.Close();
      gitProcess.StandardError.Close();
    };
  }
}
