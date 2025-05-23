#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["clientSignalR/clientSignalR.csproj", "clientSignalR/"]
COPY ["sharedCore/sharedCore.csproj", "sharedCore/"]
RUN dotnet restore "./clientSignalR/./clientSignalR.csproj"
COPY . .
WORKDIR "/src/clientSignalR"
RUN dotnet build "./clientSignalR.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./clientSignalR.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "clientSignalR.dll"]