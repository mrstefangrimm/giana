using Giana.Api.Analysis.Ranking;
using Giana.Api.Core.Fluent;
using Giana.Api.Load;
using System;

const string gitExePath = @"C:\Program Files\Git\bin\git.exe";
const string gitRepository = "https://github.com/dotnet/wpf.git";

var repo = await GitRepository.CreateAsync(gitRepository, gitExePath);

var records = await repo.LogAsync();

records = records
  .TimeRange().In(DateTime.Now.AddMonths(-6), DateTime.Now)
  .Rename().Author("Thomas Goulet", "ThomasGoulet73")
  .Build();

var authorRanking = AuthorRankingCalculations.CreateAuthorRankingSorted(records);
AuthorRankingActions.WriteAsCsv(authorRanking, Console.Out);
