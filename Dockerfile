FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY AquariumManager.sln ./
COPY src/AquariumManager.Domain/AquariumManager.Domain.csproj src/AquariumManager.Domain/
COPY src/AquariumManager.Application/AquariumManager.Application.csproj src/AquariumManager.Application/
COPY src/AquariumManager.Infrastructure/AquariumManager.Infrastructure.csproj src/AquariumManager.Infrastructure/
COPY src/AquariumManager.Api/AquariumManager.Api.csproj src/AquariumManager.Api/

RUN dotnet restore src/AquariumManager.Api/AquariumManager.Api.csproj

COPY src ./src

RUN dotnet build src/AquariumManager.Api/AquariumManager.Api.csproj -c Release --no-restore
RUN dotnet publish src/AquariumManager.Api/AquariumManager.Api.csproj -c Release -o /app/publish --no-build /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "AquariumManager.Api.dll"]