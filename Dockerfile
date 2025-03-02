FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Instalacja narzędzi EF Core
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

# Kopiowanie projektów
COPY ["src/Articles.API/Articles.API.csproj", "Articles.API/"]
COPY ["src/Articles.Application/Articles.Application.csproj", "Articles.Application/"]
COPY ["src/Articles.Domain/Articles.Domain.csproj", "Articles.Domain/"]
COPY ["src/Articles.Infrastructure/Articles.Infrastructure.csproj", "Articles.Infrastructure/"]

# Restore
RUN dotnet restore "Articles.API/Articles.API.csproj"

# Kopiowanie kodu źródłowego
COPY ["src/", "./"]

# Build
RUN dotnet build "Articles.API/Articles.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Articles.API/Articles.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Articles.API.dll"]