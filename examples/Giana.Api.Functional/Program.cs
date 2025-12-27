using Giana.Api.Analysis.Ranking;
using Giana.Api.Core;
using Giana.Api.Load;
using System;

const string gitExePath = @"C:\Program Files\Git\bin\git.exe";
const string gitRepository = "https://github.com/dotnet/wpf.git";

string tempClone = await Actions.CreateCloneFromUriAsync(gitExePath, gitRepository);
string repoName = await Actions.RequestRepositoryNameAsync(gitExePath, tempClone);

var records = await Actions.RequestGitLogAsync(gitExePath, repoName, tempClone);

records = records.WithTimeRange(DateTime.Now.AddMonths(-6), DateTime.Now);
records = records.RenameAuthor("Thomas Goulet", "ThomasGoulet73");

var authorRanking = AuthorRankingCalculations.CreateAuthorRankingSorted(records);
AuthorRankingActions.WriteAsCsv(authorRanking, Console.Out);
