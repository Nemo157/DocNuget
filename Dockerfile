FROM microsoft/aspnet
MAINTAINER Wim Looman <wim@nemo157.com>

COPY . /source

RUN cd /source && Configuration=Release ./build.sh dnu-publish && mv /source/artifacts/site /site

EXPOSE 80
WORKDIR /site
ENTRYPOINT ["/bin/bash", "kestrel", "--server.urls", "http://localhost:80"]
