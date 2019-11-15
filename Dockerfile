FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-buster

RUN apt update \
    && apt install -y git mono-complete \
    && dotnet tool install --global PowerShell

# RUN git clone --recurse-submodules--single-branch --branch updateLinuxBuild -j8 https://github.com/techtalk/SpecFlow.git /src \
#     && ls -la

WORKDIR /src

COPY . .

RUN git clean -fdx

# build project
# RUN pwsh /src/build.ps1

RUN pwsh /src/build.ps1 \
    && ls /src/SpecFlow.Tools.MsBuild.Generation/bin/Debug/netcoreapp2.0 -la

#CMD /bin/sh
CMD dotnet test /src/*.sln -v n --no-build --logger "trx;LogFileName=TestResults.trx"
