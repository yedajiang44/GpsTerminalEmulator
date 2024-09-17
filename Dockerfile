FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY .output/linux-x64/ .
EXPOSE 8080
ENV LANG=en_US.UTF-8
ENV TZ=Asia/Shanghai
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENTRYPOINT ["./Jt808TerminalEmulator.Api"] 