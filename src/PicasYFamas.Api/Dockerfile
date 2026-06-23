# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/PicasYFamas.Api/PicasYFamas.Api.csproj", "src/PicasYFamas.Api/"]
COPY ["src/PicasYFamas.Application/PicasYFamas.Application.csproj", "src/PicasYFamas.Application/"]
COPY ["src/PicasYFamas.Domain/PicasYFamas.Domain.csproj", "src/PicasYFamas.Domain/"]
COPY ["src/PicasYFamas.Infrastructure/PicasYFamas.Infrastructure.csproj", "src/PicasYFamas.Infrastructure/"]
RUN dotnet restore "./src/PicasYFamas.Api/PicasYFamas.Api.csproj"
COPY . .
WORKDIR "/src/src/PicasYFamas.Api"
RUN dotnet build "./PicasYFamas.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PicasYFamas.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PicasYFamas.Api.dll"]
