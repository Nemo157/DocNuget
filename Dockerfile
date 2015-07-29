FROM microsoft/aspnet
MAINTAINER Wim Looman <wim@nemo157.com>

COPY . /source

RUN \
  apt-get update && \
  apt-get install -y node && \
  cd /source && \
  Configuration=Release ./build.sh dnu-publish && \
  mv /source/artifacts/site /site && \
  rm /source

EXPOSE 80
WORKDIR /site
ENTRYPOINT ["/bin/bash", "kestrel", "--server.urls", "http://localhost:80"]
