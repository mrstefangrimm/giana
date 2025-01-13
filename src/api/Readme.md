# Giana.Api readme

## Functional API

```csharp
const string gitExePath = @"C:\Program Files\Git\bin\git.exe";
const string gitRepository = "https://github.com/dotnet/wpf.git";

string tempClone = Actions.CreateCloneFromUri(gitRepository, gitExePath);
string repoName = Actions.RequestRepositoryName(tempClone, gitExePath);

var records = Actions.RequestGitLog(tempClone, repoName, gitExePath);

records = records.WithTimeRange(DateTime.Now.AddMonths(-6), DateTime.Now);
records = records.RenameAuthor("Thomas Goulet", "ThomasGoulet73");

var authorRanking = AuthorRankingCalculations.CreateAuthorRankingSorted(records);
AuthorRankingActions.WriteAsCsv(authorRanking, Console.Out);
```

Output, 13. Jan 2025
```
Author,FileTouches
Jeremy Kuhne,3994
Harshit,927
h3xds1nz,593
dotnet-maestro[bot],498
Thomas Goulet,349
Himanshi Goyal,159
Dipesh Kumar,157
Rishabh Chauhan,92
Ashish Kumar Singh,60
Adam Sitnik,59
Hugh Bellamy,51
Sia Gupta,40
...
```

## Fluent API

```csharp
const string gitExePath = @"C:\Program Files\Git\bin\git.exe";
const string gitRepository = "https://github.com/dotnet/wpf.git";

var repo = await GitRepository.CreateAsync(gitRepository, gitExePath);

var records = await repo.LogAsync();

records = await records
  .TimeRange().In(DateTime.Now.AddMonths(-6), DateTime.Now)
  .Rename().Author("Thomas Goulet", "ThomasGoulet73")
  .BuildAsync();

var authorRanking = AuthorRankingCalculations.CreateAuthorRankingSorted(records);
AuthorRankingActions.WriteAsCsv(authorRanking, Console.Out);
```

Output, 13. Jan 2025
```
Author,FileTouches
Jeremy Kuhne,3994
Harshit,927
h3xds1nz,593
dotnet-maestro[bot],498
Thomas Goulet,349
Himanshi Goyal,159
Dipesh Kumar,157
Rishabh Chauhan,92
Ashish Kumar Singh,60
Adam Sitnik,59
Hugh Bellamy,51
Sia Gupta,40
...
```
