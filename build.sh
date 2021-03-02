dotnet publish ./src/Jt808TerminalEmulator.Api/Jt808TerminalEmulator.Api.csproj -c Release
docker build --pull --rm --no-cache -f "Dockerfile" -t yedajiang44/jt808terminalemulator "."