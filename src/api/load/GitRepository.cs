﻿using Giana.Api.Core;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using File = System.IO.File;

namespace Giana.Api.Load;

public sealed class GitRepository : IDisposable
{
  private readonly string _localPath;
  private readonly bool _isTempDir;
  private readonly string _repoName;
  private readonly string _gitExePath;

  public void Dispose()
  {
    if (_isTempDir)
    {
      RemoveReadOnly(_localPath);

      Directory.Delete(_localPath, true);
    }
    GC.SuppressFinalize(this);
  }

  public static GitRepository Create(string localPathOrUri, string gitExePath)
  {
    if (localPathOrUri.StartsWith("https"))
    {
      var tempDir = Actions.CreateCloneFromUri(localPathOrUri, gitExePath);
      var repoNameFromTemp = Actions.RequestRepositoryName(tempDir, gitExePath);

      return new GitRepository(tempDir, true, repoNameFromTemp, gitExePath);
    }

    var repoName = Actions.RequestRepositoryName(localPathOrUri, gitExePath);
    return new GitRepository(localPathOrUri, false, repoName, gitExePath);
  }

  public static Task<GitRepository> CreateAsync(string localPathOrUri, string gitExePath)
  {
    return Task.Factory.StartNew(() => Create(localPathOrUri, gitExePath));
  }

  public IImmutableList<GitLogRecord> Log(DateTime? deadline = null)
  {
    return Actions.RequestGitLog(_localPath, _repoName, _gitExePath, deadline);
  }

  public Task<IImmutableList<GitLogRecord>> LogAsync(DateTime? deadline = null)
  {
    return Task.Factory.StartNew(() => Log(deadline));
  }

  public LazyRecords<GitLogRecord> LogLazy(DateTime? deadline = null)
  {
    return new LazyRecords<GitLogRecord>(() => Log(deadline));
  }

  public IImmutableList<string> ActiveNames()
  {
    return Actions.RequestActiveNamesFromMainBranch(_localPath, _gitExePath);
  }

  public Task<IImmutableList<string>> ActiveNamesAsync()
  {
    return Task.Factory.StartNew(ActiveNames);
  }

  private GitRepository(string path, bool isTempDir, string repoName, string gitExePath)
  {
    _localPath = path;
    _isTempDir = isTempDir;
    _repoName = repoName;
    _gitExePath = gitExePath;
  }

  private static void RemoveReadOnly(string dir)
  {
    var subDirs = Directory.GetDirectories(dir);
    foreach (var subDir in subDirs)
    {
      RemoveReadOnly(subDir);
    }
    var files = Directory.GetFiles(dir);
    foreach (var file in files)
    {
      File.SetAttributes(file, File.GetAttributes(file) & ~FileAttributes.ReadOnly);
    }
  }
}
