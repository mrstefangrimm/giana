FROM mcr.microsoft.com/dotnet/runtime:10.0

ARG RELEASE_VERSION=v0.0.8
ARG BUILD_VERSION=0.0.10

RUN apt update && \
    apt install -y git unzip vim && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

ADD "https://github.com/mrstefangrimm/giana/releases/download/$RELEASE_VERSION/Giana.App.Cmd.$BUILD_VERSION.zip" /app
RUN unzip "Giana.App.Cmd.$BUILD_VERSION.zip"
RUN chmod 766 ./Giana.App.Cmd.sh
RUN rm "Giana.App.Cmd.$BUILD_VERSION.zip"

ENV GitExePath=/usr/bin/git

# Optional entry point
#ENTRYPOINT ["dotnet", "Giana.App.Cmd.dll"]
# or
#ENTRYPOINT ["./Giana.App.Cmd.sh", ""]
