echo off

export GitExePath=/usr/bin/git

dotnet Giana.App.Cmd.dll $1 $2 $3 $4
