FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Articles.sln", "./"]
COPY ["src/Articles.API/Articles.API.csproj", "src/Articles.API/"]
COPY ["src/Articles.Application/Articles.Application.csproj", "src/Articles.Application/"]
COPY ["src/Articles.Domain/Articles.Domain.csproj", "src/Articles.Domain/"]
COPY ["src/Articles.Infrastructure/Articles.Infrastructure.csproj", "src/Articles.Infrastructure/"]
RUN dotnet restore
COPY . .
WORKDIR "/src/src/Articles.API"
RUN dotnet build "Articles.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Articles.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Articles.API.dll"]