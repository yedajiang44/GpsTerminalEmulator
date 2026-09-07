FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src
COPY . .
RUN dotnet publish src/Jt808TerminalEmulator.Api/Jt808TerminalEmulator.Api.csproj \
    -c Release -o /app/publish \
    --self-contained false \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV LANG=en_US.UTF-8
ENV TZ=Asia/Shanghai
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Jt808TerminalEmulator.Api.dll"]