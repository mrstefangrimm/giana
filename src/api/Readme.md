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

Output, 26. Jan 2025
```
Author,FileTouches
Jeremy Kuhne,3994
Harshit,927
h3xds1nz,664
dotnet-maestro[bot],474
Thomas Goulet,343
Hugh Bellamy,147
Himanshi Goyal,141
Dipesh Kumar,113
Adam Sitnik,59
Sia Gupta,48
Ashish Kumar Singh,45
Pankaj Chaurasia,12
Anjali,12
Eric StJohn,12
Kuldeep,8
Mitch Razga,5
Loni Tra,5
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

Output, 26. Jan 2025
```
Author,FileTouches
Jeremy Kuhne,3994
Harshit,927
h3xds1nz,664
dotnet-maestro[bot],474
Thomas Goulet,343
Hugh Bellamy,147
Himanshi Goyal,141
Dipesh Kumar,113
Adam Sitnik,59
Sia Gupta,48
Ashish Kumar Singh,45
Pankaj Chaurasia,12
Anjali,12
Eric StJohn,12
Kuldeep,8
Mitch Razga,5
Loni Tra,5
...
```
