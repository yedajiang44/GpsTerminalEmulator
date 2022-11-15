FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY ./src/Jt808TerminalEmulator.Api/bin/Release/net6.0/publish . 
#设置端口
EXPOSE 80
ENTRYPOINT ["dotnet", "Jt808TerminalEmulator.Api.dll"] 