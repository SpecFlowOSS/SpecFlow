FROM mcr.microsoft.com/dotnet/core/sdk:3.1.100-buster

RUN apt update \
    && apt install -y git mono-complete \
    && dotnet tool install --global PowerShell

RUN wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt update \
    && apt install apt-transport-https -y \
    && apt update
RUN apt install dotnet-sdk-2.1 -y

# RUN git clone --recurse-submodules--single-branch --branch updateLinuxBuild -j8 https://github.com/techtalk/SpecFlow.git /src \
#     && ls -la

WORKDIR /src

COPY . .

RUN git clean -fdx

# build project
# RUN pwsh /src/build.ps1

RUN pwsh /src/build.ps1

#CMD /bin/sh
# CMD dotnet test /src/*.sln -v n --no-build --logger "trx;LogFileName=TestResults.trx"
# CMD dotnet test ./Tests/TechTalk.SpecFlow.Specs/TechTalk.SpecFlow.Specs.csproj -v n --no-build --logger "trx;LogFileName=TestResults.trx" --filter "BasicScenarioExecutionFeature_MSTest"
# CMD dotnet test TechTalk.SpecFlow.sln -v n --no-build --logger "trx;LogFileName=TestResults.trx" --filter "BasicScenarioExecutionFeature_MSTest" && /bin/sh
CMD /bin/sh
