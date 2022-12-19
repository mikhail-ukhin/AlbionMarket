#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AlbionMarket.Api/AlbionMarket.Api.csproj", "AlbionMarket.Api/"]
RUN dotnet restore "AlbionMarket.Api/AlbionMarket.Api.csproj"
COPY . .
WORKDIR "/src/AlbionMarket.Api"
RUN dotnet build "AlbionMarket.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AlbionMarket.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AlbionMarket.Api.dll"]