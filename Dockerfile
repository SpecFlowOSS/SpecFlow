FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-alpine3.9

WORKDIR /src

COPY . .

# install mono
RUN echo "@testing http://dl-4.alpinelinux.org/alpine/edge/testing" >> /etc/apk/repositories \
  && apk update \
  && apk add --update mono@testing \
  && rm -rf /var/cache/apk/*

# build test project
RUN Build/build.ps1

#CMD /bin/sh
CMD dotnet test /src/*.sln -v n --no-build --logger "trx;LogFileName=TestResults.trx"
