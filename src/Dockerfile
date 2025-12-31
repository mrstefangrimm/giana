# docker build -f .\src\Dockerfile -t giana:latest .
# docker run -it --rm  giana:latest

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./src .
RUN dotnet publish ./app/cmd/Giana.App.Cmd.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
ENV GitExePath=/usr/bin/git
COPY --from=build /app/publish .
RUN apt update && \
    apt install -y git vim && \
    rm -rf /var/lib/apt/lists/* && \
    chmod 766 ./Giana.App.Cmd.sh

ENTRYPOINT ["/bin/bash"]    