FROM microsoft/aspnet
MAINTAINER Wim Looman <wim@nemo157.com>

COPY artifacts/DocNuget /DocNuget

EXPOSE 80
WORKDIR /DocNuget
ENTRYPOINT ["/bin/bash", "./kestrel", "--server.urls", "http://localhost:80"]
