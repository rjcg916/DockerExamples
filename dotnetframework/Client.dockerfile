FROM microsoft/windowsservercore
COPY MagicEightBallServiceClient/bin/release/ /root/
ENTRYPOINT /root/MagicEightBallServiceClient.exe