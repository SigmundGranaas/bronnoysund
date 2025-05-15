FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

COPY BrønnApi.csproj .
RUN dotnet restore BrønnApi.csproj

COPY . .

RUN dotnet publish BrønnApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BrønnApi.dll"]