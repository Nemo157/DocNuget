FROM microsoft/aspnet
MAINTAINER Wim Looman <wim@nemo157.com>

COPY . /source

RUN \
  apt-get update && \
  apt-get install -y curl && \
  curl --silent --location https://deb.nodesource.com/setup_0.12 | bash - && \
  apt-get update && \
  apt-get install -y nodejs && \
  cd /source && \
  DNX_FEED=https://www.nuget.org/api/v2 Configuration=Release ./build.sh dnu-publish && \
  mv /source/artifacts/site /site && \
  rm /source

EXPOSE 80
WORKDIR /site
ENTRYPOINT ["/bin/bash", "kestrel", "--server.urls", "http://localhost:80"]
