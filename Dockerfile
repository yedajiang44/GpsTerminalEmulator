FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY ./src/Jt808TerminalEmulator.Api/bin/Release/net5.0/publish . 
#设置端口
EXPOSE 5000
ENTRYPOINT ["dotnet", "Jt808TerminalEmulator.Api.dll"] 