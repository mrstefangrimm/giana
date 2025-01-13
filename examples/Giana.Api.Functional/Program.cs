using Giana.Api.Analysis.Ranking;
using Giana.Api.Core;
using Giana.Api.Load;
using System;

const string gitExePath = @"C:\Program Files\Git\bin\git.exe";
const string gitRepository = "https://github.com/dotnet/wpf.git";

string tempClone = Actions.CreateCloneFromUri(gitRepository, gitExePath);
string repoName = Actions.RequestRepositoryName(tempClone, gitExePath);

var records = Actions.RequestGitLog(tempClone, repoName, gitExePath);

records = records.WithTimeRange(DateTime.Now.AddMonths(-6), DateTime.Now);
records = records.RenameAuthor("Thomas Goulet", "ThomasGoulet73");

var authorRanking = AuthorRankingCalculations.CreateAuthorRankingSorted(records);
AuthorRankingActions.WriteAsCsv(authorRanking, Console.Out);
