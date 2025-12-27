// Ignore Spelling: exe
// Ignore Spelling: uri

using Giana.Api.Core;
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
  public static ImmutableList<GitLogRecord> RequestGitLog(string gitExePath, string repositoryName, string repositoryRoot, DateTime? commitsSince = default)
  {
    return GitLog(gitExePath, repositoryName, repositoryRoot, commitsSince, CancellationToken.None);
  }

  public static Task<ImmutableList<GitLogRecord>> RequestGitLogAsync(string gitExePath, string repositoryName, string repositoryRoot, DateTime? commitsSince = default, CancellationToken cancellationToken = default)
  {
    return Task.Run(() => GitLog(gitExePath, repositoryName, repositoryRoot, commitsSince, cancellationToken), cancellationToken);
  }

  private static ImmutableList<GitLogRecord> GitLog(string gitExePath, string repositoryName, string repositoryRoot, DateTime? commitsSince, CancellationToken cancellationToken)
  {
    ArgumentException.ThrowIfNullOrEmpty(gitExePath);
    ArgumentException.ThrowIfNullOrEmpty(repositoryName);
    ArgumentException.ThrowIfNullOrEmpty(repositoryRoot);
    ArgumentNullException.ThrowIfNull(cancellationToken);

    const string GitLogCmd = "log --pretty=format:\"%h^%an^%as^%s\" --date-order --name-status";

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

      // Read status lines of changed files of the commit.
      var statusLine = gitProcess.StandardOutput.ReadLine();
      var changeFileElements = statusLine.Split("\t");
      do
      {
        if (changeFileElements.Count() == 2 || changeFileElements.Count() == 3)
        {
          GitLogRecord change = new(
            RepoName: repositoryName,
            Name: changeFileElements.Last(),
            Commit: elements[0],
            Author: elements[1],
            Message: elements[3],
            Date: DateTime.Parse(elements[2]));

          records.Add(change);
        }
        else
        {
          // Commit is without changed files - the status line is the next commit.
          elements = statusLine.Split("^");

          if (elements.Length > 4)
          {
            string[] msgElements = new string[elements.Length - 3];
            Array.Copy(elements, 3, msgElements, 0, msgElements.Length);
            elements[3] = string.Join("^", msgElements);
          }
        }
        cancellationToken.ThrowIfCancellationRequested(CloseOutputStreams(gitProcess));

        statusLine = gitProcess.StandardOutput.ReadLine();

        if (statusLine != null)
        {
          changeFileElements = statusLine.Split("\t");
        }

      } while (!string.IsNullOrEmpty(statusLine));

      if (commitsSince.HasValue && records.Last().Date < commitsSince.Value)
      {
        CloseOutputStreams(gitProcess)();
        break;
      }
    }

    return records.ToImmutableList();
  }


  public static ImmutableList<string> RequestActiveNames(string gitExePath, string repositoryRoot)
  {
    return ActiveNames(gitExePath, repositoryRoot, null, CancellationToken.None);
  }

  public static ImmutableList<string> RequestActiveNamesFromBranch(string gitExePath, string repositoryRoot, string branch)
  {
    return ActiveNames(gitExePath, repositoryRoot, branch, CancellationToken.None);
  }

  public static Task<ImmutableList<string>> RequestActiveNamesAsync(string gitExePath, string repositoryRoot, CancellationToken cancellationToken = default)
  {
    return Task.Run(() => ActiveNames(gitExePath, repositoryRoot, null, cancellationToken));
  }

  public static Task<ImmutableList<string>> RequestActiveNamesFromBranchAsync(string gitExePath, string repositoryRoot, string branch, CancellationToken cancellationToken = default)
  {
    return Task.Run(() => ActiveNames(gitExePath, repositoryRoot, branch, cancellationToken));
  }

  private static ImmutableList<string> ActiveNames(string gitExePath, string repositoryRoot, string branch, CancellationToken cancellationToken)
  {
    ArgumentException.ThrowIfNullOrEmpty(gitExePath);
    ArgumentException.ThrowIfNullOrEmpty(repositoryRoot);
    ArgumentNullException.ThrowIfNull(cancellationToken);

    if (string.IsNullOrEmpty(branch))
    {
      branch = RequestMainBranchName(gitExePath, repositoryRoot, cancellationToken);
    }
    string gitCmd = $"ls-tree -r {branch} --name-only";

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

  private static string RequestMainBranchName(string gitExePath, string repositoryRoot, CancellationToken cancellationToken)
  {
    const string GitBranchCmd = "symbolic-ref refs/remotes/origin/HEAD";

    (Process gitProcess, Action defering) = CreateAndStartGitProcess(repositoryRoot, gitExePath, GitBranchCmd);
    using var defer = new Defer(defering);
    CheckStdErrOutput(gitProcess);

    while (!gitProcess.StandardOutput.EndOfStream)
    {
      cancellationToken.ThrowIfCancellationRequested(CloseOutputStreams(gitProcess));

      var line = gitProcess.StandardOutput.ReadLine();
      var path = line.Split('/');
      if (path.Length == 4)
      {
        return path[3];
      }
    }

    throw new InvalidOperationException("repository main branch not found with `git branch`");
  }


  public static string RequestRepositoryName(string gitExePath, string repositoryRoot)
  {
    return RepositoryName(gitExePath, repositoryRoot, CancellationToken.None);
  }

  public static Task<string> RequestRepositoryNameAsync(string gitExePath, string repositoryRoot, CancellationToken cancellationToken = default)
  {
    return Task.Run(() => RepositoryName(gitExePath, repositoryRoot, cancellationToken));
  }

  private static string RepositoryName(string gitExePath, string repositoryRoot, CancellationToken cancellationToken)
  {
    ArgumentException.ThrowIfNullOrEmpty(gitExePath);
    ArgumentException.ThrowIfNullOrEmpty(repositoryRoot);
    ArgumentNullException.ThrowIfNull(cancellationToken);

    const string GitRemoteCmd = "remote -v";

    (Process gitProcess, Action defering) = CreateAndStartGitProcess(repositoryRoot, gitExePath, GitRemoteCmd);
    using var defer = new Defer(defering);
    CheckStdErrOutput(gitProcess);

    while (!gitProcess.StandardOutput.EndOfStream)
    {
      cancellationToken.ThrowIfCancellationRequested(CloseOutputStreams(gitProcess));

      var line = gitProcess.StandardOutput.ReadLine();

      // Supported formats:
      // https://tfs-app.company.com/A/B/_git/giana
      // https://github.com/mrstefangrimm/giana.git (fetch) [blob:none]
      // git@gitlab.A/B/giana.git (fetch)
      var elements = line.Split('/');
      var lastElement = elements[elements.Length - 1];

      if (!lastElement.Contains(".git"))
      {
        // Assume TFS
        return lastElement;
      }
      if (lastElement.Contains(".git (fetch)"))
      {
        return lastElement.Split(".git (fetch)")[0];
      }
    }

    throw new InvalidOperationException("repository name not found with `git remote`");
  }


  public static string CreateCloneFromUri(string gitExePath, string uri)
  {
    return CloneFromUri(gitExePath, uri, null);
  }

  public static string CreateCloneFromUriFromBranch(string gitExePath, string uri, string branch)
  {
    return CloneFromUri(gitExePath, uri, branch);
  }

  public static Task<string> CreateCloneFromUriAsync(string gitExePath, string uri, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(cancellationToken);

    return Task.Run(() => CloneFromUri(gitExePath, uri, null), cancellationToken);
  }

  public static Task<string> CreateCloneFromUriFromBranchAsync(string gitExePath, string uri, string branch, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(cancellationToken);

    return Task.Run(() => CloneFromUri(gitExePath, uri, branch), cancellationToken);
  }

  private static string CloneFromUri(string gitExePath, string uri, string branch)
  {
    ArgumentException.ThrowIfNullOrEmpty(gitExePath);
    ArgumentException.ThrowIfNullOrEmpty(uri);

    var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    Directory.CreateDirectory(tempPath);

    var branchArg = branch == null ? "--single-branch" : $"--branch {branch}";

    var gitCloneCmd = $"clone --filter=blob:none --no-checkout {branchArg} {uri} {tempPath}";

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
