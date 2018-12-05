FROM microsoft/windowsservercore
COPY Legacy.Runner/bin/release/ /root/
ENTRYPOINT /root/Legacy.Runner.exe