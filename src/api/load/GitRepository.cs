// Ignore Spelling: exe
// Ignore Spelling: uri

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
  private readonly string _branch;
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

  public static GitRepository Create(string gitExePath, string localCloneOrUri)
  {
    if (!Directory.Exists(localCloneOrUri))
    {
      var tempDir = Actions.CreateCloneFromUri(gitExePath, localCloneOrUri);
      var repoNameFromTemp = Actions.RequestRepositoryName(gitExePath, tempDir);

      return new GitRepository(tempDir, true, repoNameFromTemp, null, gitExePath);
    }

    var repoName = Actions.RequestRepositoryName(gitExePath, localCloneOrUri);
    return new GitRepository(localCloneOrUri, false, repoName, null, gitExePath);
  }

  public static GitRepository CreateFromBranch(string gitExePath, string localCloneOrUri, string branch)
  {
    if (!Directory.Exists(localCloneOrUri))
    {
      var tempDir = Actions.CreateCloneFromUriFromBranch(gitExePath, localCloneOrUri, branch);
      var repoNameFromTemp = Actions.RequestRepositoryName(gitExePath, tempDir);

      return new GitRepository(tempDir, true, repoNameFromTemp, branch, gitExePath);
    }

    var repoName = Actions.RequestRepositoryName(gitExePath, localCloneOrUri);
    return new GitRepository(localCloneOrUri, false, repoName, branch, gitExePath);
  }

  public static Task<GitRepository> CreateAsync(string gitExePath, string localCloneOrUri, CancellationToken cancellationToken = default)
  {
    return Task.Run(() => Create(gitExePath, localCloneOrUri), cancellationToken);
  }

  public static Task<GitRepository> CreateFromBranchAsync(string gitExePath, string localCloneOrUri, string branch, CancellationToken cancellationToken = default)
  {
    return Task.Run(() => CreateFromBranch(gitExePath, localCloneOrUri, branch), cancellationToken);
  }

  public ImmutableList<GitLogRecord> Log(DateTime? commitsSince = null)
  {
    return Actions.RequestGitLog(_gitExePath, _repoName, _localPath, commitsSince);
  }

  public async Task<ImmutableList<GitLogRecord>> LogAsync(DateTime? commitsSince = null)
  {
    return await Actions.RequestGitLogAsync(_gitExePath, _repoName, _localPath, commitsSince, CancellationToken.None);
  }

  public async Task<ImmutableList<GitLogRecord>> LogAsync(CancellationToken cancellationToken, DateTime? commitsSince = null)
  {
    return await Actions.RequestGitLogAsync(_gitExePath, _repoName, _localPath, commitsSince, cancellationToken);
  }

  public LazyRecords<GitLogRecord> LogLazy(DateTime? commitsSince = null)
  {
    return new LazyRecords<GitLogRecord>(() => Log(commitsSince));
  }

  public ImmutableList<string> ActiveNames()
  {
    return Actions.RequestActiveNamesFromBranch(_gitExePath, _localPath, _branch);
  }

  public async Task<ImmutableList<string>> ActiveNamesAsync(CancellationToken cancellationToken = default)
  {
    return await Actions.RequestActiveNamesFromBranchAsync(_gitExePath, _localPath, _branch, cancellationToken);
  }

  private GitRepository(string path, bool isTempDir, string repoName, string branch, string gitExePath)
  {
    _localPath = path;
    _isTempDir = isTempDir;
    _repoName = repoName;
    _branch = branch;
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
