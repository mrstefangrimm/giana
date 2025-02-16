FROM registry.access.redhat.com/ubi8/dotnet-80-runtime

USER root

RUN microdnf -y install git
RUN microdnf -y install unzip

WORKDIR /app
ADD https://github.com/mrstefangrimm/giana/releases/download/v0.0.6/Giana.App.Cmd.0.0.2.zip /app
RUN unzip Giana.App.Cmd.0.0.2.zip

WORKDIR /app/Giana.App.Cmd.0.0.2
ENV GitExePath=/usr/bin/git
ENTRYPOINT ["dotnet", "Giana.App.Cmd.dll"]
