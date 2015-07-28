FROM microsoft/aspnet
MAINTAINER Wim Looman <wim@nemo157.com>

COPY artifacts/DocNuget /DocNuget

EXPOSE 5004
WORKDIR /DocNuget
ENTRYPOINT ["/bin/bash", "./kestrel"]
