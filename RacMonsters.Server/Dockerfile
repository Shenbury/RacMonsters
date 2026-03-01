# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY RacMonsters.Server/RacMonsters.Server.csproj RacMonsters.Server/
COPY RacMonsters.AppHost/RacMonsters.AppHost.csproj RacMonsters.AppHost/
COPY RacMonsters.AppHost/Projects.RacMonsters_Server.csproj Projects.RacMonsters_Server/ || true

# Copy the rest of the solution
COPY . .

WORKDIR /src/RacMonsters.Server
RUN dotnet restore "RacMonsters.Server.csproj"
RUN dotnet publish "RacMonsters.Server.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "RacMonsters.Server.dll"]
