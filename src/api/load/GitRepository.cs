using Giana.Api.Core;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
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

  public static async Task<GitRepository> CreateAsync(string localPathOrUri, string gitExePath)
  {
    if (localPathOrUri.StartsWith("https"))
    {
      var tempDir = await Actions.CreateCloneFromUriAsync(localPathOrUri, gitExePath);
      var repoNameFromTemp = await Actions.RequestRepositoryNameAsync(tempDir, gitExePath);

      return new GitRepository(tempDir, true, repoNameFromTemp, gitExePath);
    }

    var repoName = await Actions.RequestRepositoryNameAsync(localPathOrUri, gitExePath);

    return new GitRepository(localPathOrUri, false, repoName, gitExePath);
  }

  public static async Task<GitRepository> CreateAsync(string localPathOrUri, string gitExePath, CancellationToken cancellationToken)
  {
    return await Task.Run(() => Create(localPathOrUri, gitExePath), cancellationToken);
  }

  public IImmutableList<GitLogRecord> Log(DateTime? commitsFrom = null)
  {
    return Actions.RequestGitLog(_localPath, _repoName, _gitExePath, commitsFrom);
  }

  public async Task<IImmutableList<GitLogRecord>> LogAsync(DateTime? commitsFrom = null)
  {
    return await Actions.RequestGitLogAsync(_localPath, _repoName, _gitExePath, CancellationToken.None, commitsFrom);
  }

  public async Task<IImmutableList<GitLogRecord>> LogAsync(CancellationToken cancellationToken, DateTime? commitsFrom = null)
  {
    return await Actions.RequestGitLogAsync(_localPath, _repoName, _gitExePath, cancellationToken, commitsFrom);
  }

  public LazyRecords<GitLogRecord> LogLazy(DateTime? commitsFrom = null)
  {
    return new LazyRecords<GitLogRecord>(() => Log(commitsFrom));
  }

  public IImmutableList<string> ActiveNames()
  {
    return Actions.RequestActiveNamesFromMainBranch(_localPath, _gitExePath);
  }

  public async Task<IImmutableList<string>> ActiveNamesAsync()
  {
    return await Actions.RequestActiveNamesFromMainBranchAsync(_localPath, _gitExePath);
  }

  public async Task<IImmutableList<string>> ActiveNamesAsync(CancellationToken cancellationToken)
  {
    return await Actions.RequestActiveNamesFromMainBranchAsync(_localPath, _gitExePath, cancellationToken);
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
