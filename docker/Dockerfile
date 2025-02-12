
FROM mcr.microsoft.com/dotnet/runtime:8.0
RUN apt update
RUN apt install -y git unzip

WORKDIR /app
ADD https://github.com/mrstefangrimm/giana/releases/download/v0.0.6/Giana.App.Cmd.0.0.2.zip /app
RUN unzip Giana.App.Cmd.0.0.2.zip

WORKDIR /app/Giana.App.Cmd.0.0.2
ENV GitExePath=/usr/bin/git
ENTRYPOINT ["dotnet", "Giana.App.Cmd.dll"]

#RUN chmod 777 ./Giana.App.Cmd.sh
#ENTRYPOINT ["./Giana.App.Cmd.sh", ""]
