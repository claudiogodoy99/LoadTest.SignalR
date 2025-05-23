#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["clientProducerSignalR/clientProducerSignalR.csproj", "clientProducerSignalR/"]
COPY ["sharedCore/sharedCore.csproj", "sharedCore/"]
RUN dotnet restore "./clientProducerSignalR/./clientProducerSignalR.csproj"
COPY . .
WORKDIR "/src/clientProducerSignalR"
RUN dotnet build "./clientProducerSignalR.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./clientProducerSignalR.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "clientProducerSignalR.dll"]