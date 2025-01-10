using Giana.Api.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giana.Api.Load;

public static class Actions
{
  public static ImmutableList<GitLogRecord> RequestGitLog(string repositoryRoot, string repositoryName, string gitExePath, DateTime? deadline = null)
  {
    const string GIT_LOG_CMD_NEW = "log --pretty=format:\"%h^%an^%as^%s\" --name-only";

    List<GitLogRecord> changes = new();

    ProcessStartInfo gitInfo = new ProcessStartInfo();
    gitInfo.CreateNoWindow = true;
    gitInfo.RedirectStandardError = true;
    gitInfo.RedirectStandardOutput = true;
    gitInfo.FileName = gitExePath;
    gitInfo.StandardOutputEncoding = Encoding.UTF8;

    Process gitProcess = new Process();
    gitInfo.Arguments = GIT_LOG_CMD_NEW;
    gitInfo.WorkingDirectory = repositoryRoot;

    gitProcess.StartInfo = gitInfo;
    gitProcess.Start();

    var stderr_strTask = gitProcess.StandardError.ReadToEndAsync();
    bool hasWaitedTooLong = stderr_strTask.Wait(1000);

    if (stderr_strTask.Status == TaskStatus.RanToCompletion || !hasWaitedTooLong)
    {
      // TODO: Parallel-Foreach
      // https://stackoverflow.com/questions/49217299/c-sharp-parallelizing-while-loop-with-streamreader-causing-high-cpu
      //gitProcess.StandardOutput
      //  .ReadLinesAsync()
      //  .AsParallel()
      //  .ForAll(line => Console.WriteLine(line));

      while (!gitProcess.StandardOutput.EndOfStream)
      {
        // Read commit line
        var commitLine = gitProcess.StandardOutput.ReadLine();

        var elements = commitLine.Split("^");

        if (elements.Length > 4)
        {
          //Console.Error.WriteLine(commitLine);

          string[] msgElements = new string[elements.Length - 3];
          Array.Copy(elements, 3, msgElements, 0, msgElements.Length);
          elements[3] = string.Join("^", msgElements);
        }

        //List<string> fileLines = new List<string>();
        //var fileLine1 = gitProcess.StandardOutput.ReadLine();
        //do
        //{
        //  fileLines.Add(fileLine1);
        //  fileLine1 = gitProcess.StandardOutput.ReadLine();

        //} while (!string.IsNullOrEmpty(fileLine1));

        //fileLines.AsParallel().ForAll(line =>
        //{
        //  var change = new GitLogRecord(
        //    RepoName: repositoryName,
        //    Name: line,
        //    Commit: elements[0],
        //    Author: elements[1],
        //    Message: elements[3],
        //    Date: DateTime.Parse(elements[2]));

        //  changes.Add(change);
        //});
        //var fileLineTask1 = gitProcess.StandardOutput.ReadLineAsync();
        //var fileLineTask2 = gitProcess.StandardOutput.ReadLineAsync();
        //var fileLineTask3 = gitProcess.StandardOutput.ReadLineAsync();
        //var fileLineTask4 = gitProcess.StandardOutput.ReadLineAsync();
        //var fileLineTask5 = gitProcess.StandardOutput.ReadLineAsync();

        //fileLineTask1.Wait();
        //fileLineTask2.Wait();
        //fileLineTask3.Wait();
        //fileLineTask4.Wait();
        //fileLineTask5.Wait();

        // Read lines of changed files
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

          changes.Add(change);

          fileLine = gitProcess.StandardOutput.ReadLine();

        } while (!string.IsNullOrEmpty(fileLine));

        if (deadline.HasValue && changes.Last().Date < deadline.Value)
        {
          break;
        }
      }
      //string stdout_str = gitProcess.StandardOutput.ReadToEnd(); // pick up STDOUT
    }

    if (!gitProcess.HasExited)
    {
      gitProcess.WaitForExit(1000);
    }
    gitProcess.Close();

    return changes.ToImmutableList();
  }

  public static ImmutableList<string> RequestActiveNamesFromMainBranch(string repositoryRoot, string gitExe)
  {
    string mainBranch = RequestMainBranchName(repositoryRoot, gitExe);

    string gitCmd = $"ls-tree -r {mainBranch} --name-only";

    List<string> changes = new();

    ProcessStartInfo gitInfo = new ProcessStartInfo();
    gitInfo.CreateNoWindow = true;
    gitInfo.RedirectStandardError = true;
    gitInfo.RedirectStandardOutput = true;
    gitInfo.FileName = gitExe;
    gitInfo.StandardOutputEncoding = Encoding.UTF8;

    Process gitProcess = new Process();
    gitInfo.Arguments = gitCmd;
    gitInfo.WorkingDirectory = repositoryRoot;

    gitProcess.StartInfo = gitInfo;
    gitProcess.Start();

    var stderr_strTask = gitProcess.StandardError.ReadToEndAsync();
    bool hasWaitedTooLong = stderr_strTask.Wait(1000);

    if (stderr_strTask.Status == TaskStatus.RanToCompletion || !hasWaitedTooLong)
    {
      // TODO: Parallel-Foreach
      while (!gitProcess.StandardOutput.EndOfStream)
      {
        var commitLine = gitProcess.StandardOutput.ReadLine();
        changes.Add(commitLine);
      }
      //string stdout_str = gitProcess.StandardOutput.ReadToEnd(); // pick up STDOUT
    }

    gitProcess.WaitForExit();
    gitProcess.Close();

    return changes.ToImmutableList();

  }

  internal static string RequestMainBranchName(string repositoryRoot, string gitExePath)
  {
    const string GIT_BRANCH_CMD = "branch";

    ProcessStartInfo gitInfo = new ProcessStartInfo();
    gitInfo.CreateNoWindow = true;
    gitInfo.RedirectStandardError = true;
    gitInfo.RedirectStandardOutput = true;
    gitInfo.FileName = gitExePath;
    gitInfo.StandardOutputEncoding = Encoding.UTF8;

    Process gitProcess = new Process();
    gitInfo.Arguments = GIT_BRANCH_CMD;
    gitInfo.WorkingDirectory = repositoryRoot;

    gitProcess.StartInfo = gitInfo;
    gitProcess.Start();

    var stderr_strTask = gitProcess.StandardError.ReadToEndAsync();
    bool hasWaitedTooLong = stderr_strTask.Wait(1000);

    if (stderr_strTask.Status == TaskStatus.RanToCompletion || !hasWaitedTooLong)
    {
      while (!gitProcess.StandardOutput.EndOfStream)
      {
        var line = gitProcess.StandardOutput.ReadLine();

        if (line.StartsWith("* "))
        {

          return line.Remove(0, 2);
        }
      }
      //string stdout_str = gitProcess.StandardOutput.ReadToEnd(); // pick up STDOUT
    }

    return string.Empty;
  }

  public static string RequestRepositoryName(string repositoryRoot, string gitExePath)
  {
    const string GIT_REMOTE_CMD = "remote -v";

    ProcessStartInfo gitInfo = new ProcessStartInfo();
    gitInfo.CreateNoWindow = true;
    gitInfo.RedirectStandardError = true;
    gitInfo.RedirectStandardOutput = true;
    gitInfo.FileName = gitExePath;
    gitInfo.StandardOutputEncoding = Encoding.UTF8;

    Process gitProcess = new Process();
    gitInfo.Arguments = GIT_REMOTE_CMD;
    gitInfo.WorkingDirectory = repositoryRoot;

    gitProcess.StartInfo = gitInfo;
    gitProcess.Start();

    var stderr_strTask = gitProcess.StandardError.ReadToEndAsync();
    bool hasWaitedTooLong = stderr_strTask.Wait(1000);

    if (stderr_strTask.Status == TaskStatus.RanToCompletion || !hasWaitedTooLong)
    {
      while (!gitProcess.StandardOutput.EndOfStream)
      {
        var line = gitProcess.StandardOutput.ReadLine();

        // Supported formats:
        // https://tfs-app.company.com/A/B/_git/{RepoName}
        // https://github.com/dotnet/runtime.git (fetch) [blob:none]
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
      //string stdout_str = gitProcess.StandardOutput.ReadToEnd(); // pick up STDOUT
    }

    return string.Empty;
  }

  public static string CreateCloneFromUri(string uri, string gitExePath)
  {
    //var tempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Folder1");// Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    Directory.CreateDirectory(tempPath);

    string gitCmd = $"clone --filter=blob:none --no-checkout --single-branch {uri} {tempPath}";

    ProcessStartInfo gitInfo = new ProcessStartInfo();
    gitInfo.CreateNoWindow = true;
    gitInfo.RedirectStandardError = true;
    gitInfo.RedirectStandardOutput = true;
    gitInfo.FileName = gitExePath;
    gitInfo.StandardOutputEncoding = Encoding.UTF8;

    Process gitProcess = new Process();
    gitInfo.Arguments = gitCmd;
    gitInfo.WorkingDirectory = tempPath;

    gitProcess.StartInfo = gitInfo;
    gitProcess.Start();

    var stderr_strTask = gitProcess.StandardError.ReadToEndAsync();
    bool hasWaitedTooLong = stderr_strTask.Wait(1000);

    gitProcess.WaitForExit();
    gitProcess.Close();

    return tempPath;


  }

  internal static IEnumerable<string> ObsoleteParseFileSystem(string repositoryRoot)
  {
    List<string> files = new();

    ObsoleteParse(repositoryRoot, files);

    return files;
  }

  private static void ObsoleteParse(string path, List<string> paths)
  {
    foreach (var dir in Directory.GetDirectories(path))
    {
      ObsoleteParse(dir, paths);
    }

    paths.AddRange(Directory.GetFiles(path));
  }

  internal static void ObsoleteWriteHeaderAsCsvFile(TextWriter writer)
  {
    writer.WriteLine("# Repository; Commit; Date; Author; Description; Path");
  }

  internal static void ObsoleteWriteGitChangesAsCsvFile(TextWriter writer, IEnumerable<GitLogRecord> gitChanges)
  {
    foreach (var gitChange in gitChanges)
    {
      writer.WriteLine($"{gitChange.RepoName}; {gitChange.Commit}; {gitChange.Date}; {gitChange.Author}; {gitChange.Message}; {gitChange.Name}");
    }
  }

  public static IEnumerable<GitLogRecord> ObsoleteParseGitHistory(string repositoryRoot, string repositoryName, string gitExe, IEnumerable<string> repositoryFiles)
  {
    const string GIT_LOG_CMD = "log --pretty=format:\"%h; %an; %as; %s\" ";

    List<GitLogRecord> changes = new();

    foreach (var path in repositoryFiles)
    {
      ProcessStartInfo gitInfo = new ProcessStartInfo();
      gitInfo.CreateNoWindow = true;
      gitInfo.RedirectStandardError = true;
      gitInfo.RedirectStandardOutput = true;
      gitInfo.FileName = gitExe;
      gitInfo.StandardOutputEncoding = Encoding.UTF8;

      Process gitProcess = new Process();
      gitInfo.Arguments = GIT_LOG_CMD + path;
      gitInfo.WorkingDirectory = repositoryRoot;

      gitProcess.StartInfo = gitInfo;
      gitProcess.Start();

      var stderr_strTask = gitProcess.StandardError.ReadToEndAsync();
      bool hasWaitedTooLong = stderr_strTask.Wait(1000);

      if (stderr_strTask.Status == TaskStatus.RanToCompletion || !hasWaitedTooLong)
      {
        while (!gitProcess.StandardOutput.EndOfStream)
        {
          var line = gitProcess.StandardOutput.ReadLine();
          var elements = line.Split("; ");

          if (elements.Length == 4)
          {
            GitLogRecord change = new(
              RepoName: repositoryName,
              Name: path,
              Commit: elements[0],
              Author: elements[1],
              Message: elements[3],
              Date: DateTime.Parse(elements[2]));

            changes.Add(change);
            Console.WriteLine(change);
          }
          else
          {
            Console.WriteLine(line);
          }
        }
        //string stdout_str = gitProcess.StandardOutput.ReadToEnd(); // pick up STDOUT
      }

      gitProcess.WaitForExit();
      gitProcess.Close();
    }

    return changes;
  }

}
