FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY .output . 
#设置端口
EXPOSE 80
ENTRYPOINT ["dotnet", "Jt808TerminalEmulator.Api.dll"] 