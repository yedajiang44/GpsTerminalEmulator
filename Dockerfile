FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY .output . 
#设置端口
EXPOSE 80
ENTRYPOINT ["Jt808TerminalEmulator.Api"] 