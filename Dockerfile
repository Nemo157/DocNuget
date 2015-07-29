FROM microsoft/aspnet
MAINTAINER Wim Looman <wim@nemo157.com>

RUN curl --silent --location https://deb.nodesource.com/setup_0.12 | bash - && apt-get install -y nodejs

COPY . /source

RUN cd /source && Configuration=Release ./build.sh dnu-publish
RUN mv /source/artifacts/DocNuget /site && rm -rf /source

EXPOSE 80
WORKDIR /site
ENTRYPOINT ["/bin/bash", "kestrel", "--server.urls", "http://localhost:80"]
