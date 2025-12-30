# Giana.App.Cmd readme

A command line tool that uses an input file for the git repository analysis:

```sh
>.\Giana.App.Cmd.bat
usage: Giana.Cmd.App.[bat|sh] [(-q | --query-file) <filename>] [(-o | output-file) <filename>]

--query-file    json file with the query arguments. By default, query.json for the bin folder is used.
--output-file   csv filename for the results. By default, the results are written to the standard output.
```



An example input file for the command line tool. All commits since 2024-01-01 and within the time ranges 2024-01-01 to 2024-07-01 and 2025-02-01 to 2025-04-30 are analysed in the repositories giana and parc. The analysis only includes *.cs, *.h and *.cpp files; the two commits 5a6678c and aa3acf4 are excluded though.

```json
// query.json
{
  "sources": [ "https://github.com/mrstefangrimm/giana.git", "https://github.com/mrstefangrimm/parc.git" ],
  "analyzer": "file-ranking",
  "outputformat": "csv",
  "timeranges": [
    {
      "begin": "2024-01-01",
      "end": "2024-07-01"
    },
    {
      "begin": "2025-02-01",
      "end": "2025-04-30"
    }
  ],
  "renames": [],
  "includes": {
    "names": [ "\\.cs$", "\\.h$", "\\.cpp$" ],
    "commits": [],
    "authors": [],
    "messages": []
  },
  "excludes": {
    "names": [],
    "commits": [ "5a6678c", "aa3acf4" ],
    "authors": [],
    "messages": []
  },
  "commitsSince": "2024-01-01"
}
```

