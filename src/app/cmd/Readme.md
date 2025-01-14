# Giana.App.Cmd readme

Command line:

```sh
>./Giana.App.Cmd.sh
usage: Giana.Cmd.App.[bat|sh] [(-q | --query-file) <filename>] [(-o | output-file) <filename>]

--query-file    json file with the query arguments. By default, query.json for the bin folder is used.
--output-file   csv filename for the results. By default, the results are written to the standard output.
```



query.json

```json
{
  "sources": [ "https://github.com/mrstefangrimm/giana.git", "https://github.com/mrstefangrimm/parc.git" ],
  "analyzer": "file-ranking",
  "outputformat": "csv",
  "timeranges": [],
  "renames": [],
  "includes": {
    "names": [".*.cs", ".*.[h|cpp]"],
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
  "commitsFrom": ""
}
```

