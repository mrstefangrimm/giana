[![.NET](https://github.com/mrstefangrimm/giana/actions/workflows/ci.yml/badge.svg)](https://github.com/mrstefangrimm/giana/actions/workflows/ci.yml)

# giana

giana stands of GIt ANAlysis. Giana uses `git log` to extract and process the data.

giana is written in C# and includes:
 - A command line tool (`Giana.App.Cmd.exe`)
 - A public API (Giana.Api nuget package) that you can use in a C# project


## Key feature

Giana analyzers calculate coupling and cohesion between files. It quickly becomes clear where the software design needs to be improved. Filters allow you to analyze specific areas of your source code.

### Example when and how to use giana

Imagine a repository where you defined an `IConfig` interface for various supported devices:

```
Shared\Config\IConfig.cs
Devices\Mouse\MouseConfig.cs
Devices\Keyboard\KeyboardConfig.cs
Devices\Speaker\SpeakerConfig.cs
```

After some time you do an file-ranking and a file-coupling analysis.

#### 1. file-ranking analysis across the entire repository.

Command line:
`>  .\Giana.App.Cmd.bat -q file-ranking.json`

Query file file-ranking.json:
```json
{
  "sources": [ "https://github.com/imagine/this-repository.git" ],
  "analyzer": "file-ranking",
  "outputformat": "csv",
  "timeranges": [],
  "renames": [],
  "includes": {
    "names": [],
    "commits": [],
    "authors": [],
    "messages": []
  },
  "excludes": {
    "names": [],
    "commits": [],
    "authors": [],
    "messages": []
  }
}
```

Result:
| Name | Changes |
| --- | --- |
|Devices\Keyboard\KeyboardConfig.cs|153|
|Devices\Mouse\MouseConfig.cs|132|
|Devices\Speaker\SpeakerConfig.cs|121|
|Shared\Config\IConfig.cs|105|
|*...*|*...*|
  > The device configuration files were the most frequently changed in the repository.

#### 2. file-coupling analysis which includes files in Shared\Config\\* and Devices\\*

Command line:
`>  .\Giana.App.Cmd.bat -q file-coupling.json`

Query file file-ranking.json:
```json
{
  "sources": [ "https://github.com/imagine/this-repository.git" ],
  "analyzer": "file-coupling",
  "outputformat": "csv",
  "timeranges": [],
  "renames": [],
  "includes": {
    "names": [ "Shared/Config/*", "Devices/*" ],
    "commits": [],
    "authors": [],
    "messages": []
  },
  "excludes": {
    "names": [],
    "commits": [],
    "authors": [],
    "messages": []
  }
}
```

Result:
| Name1 | Name2 | Changes |
| --- | --- | --- |
|Devices\Speaker\SpeakerConfig.cs|Devices\Mouse\MouseConfig.cs|108|
|Devices\Keyboard\KeyboardConfig.cs|Devices\Speaker\SpeakerConfig.cs|98|
|Devices\Keyboard\KeyboardConfig.cs|Devices\Mouse\MouseConfig.cs|95|
|Shared\Config\IConfig.cs|Devices\Mouse\MouseConfig.cs|91|
|Shared\Config\IConfig.cs|Devices\Speaker\SpeakerConfig.cs|88|
|Shared\Config\IConfig.cs|Devices\Keyboard\KeyboardConfig.cs|82|
|*...*|*...*|*...*|
  > The implementations have a strong coupling with each other and with the interface.


This simple analysis reveals a weak point: the three device implementations and the interface are highly coupled and have a high volatility.
Was this shared interface a good choice? If a forth device implementation was added, things would get even worse.

## Where to get giana

 - Giana.API Nuget: https://www.nuget.org/packages/Giana.Api
 - Source code: https://github.com/mrstefangrimm/giana
 - Command-line tool: Currently you have to get the source code and build it yourself
