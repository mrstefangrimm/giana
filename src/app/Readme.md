# Giana.App readme


```csharp
const string gitExePath = @"C:\Program Files\Git\bin\git.exe";
const string gitRepository = "https://github.com/dotnet/wpf.git";

var query = new Query
{
  Sources = [gitRepository],
  Analyzer = "author-ranking",
  OutputFormat = "csv",
  Renames = [new Author() { To = "Thomas Goulet", From = "ThomasGoulet73" }],
  TimeRanges = [new TimePeriod() { Begin = DateTime.Now.AddMonths(-6), End = DateTime.Now }]
};

var routine = query.CreateRoutine();

await routine.ExecuteAsync(gitExePath, Console.Out, TimeSpan.FromSeconds(60));
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
