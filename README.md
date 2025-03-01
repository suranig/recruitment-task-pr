# Articles API

Projekt API do zarządzania artykułami, implementujący wzorzec Clean Architecture w ASP.NET Core 8.0.

## Wymagania

- .NET 8.0 SDK
- (opcjonalnie) Docker i Docker Compose

## Uruchamianie projektu lokalnie

### Przez .NET CLI

# Przywróć zależności
dotnet restore

# Zbuduj projekt
dotnet build

# Uruchom API
dotnet run --project src/Articles.API/Articles.API.csproj

API będzie dostępne pod adresem:
- HTTP: http://localhost:5021

### Przez Docker (opcjonalnie)

```
docker-compose up -d
```

## Uruchamianie testów

### Testy jednostkowe

```
dotnet test tests/Articles.UnitTests/Articles.UnitTests.csproj
```

### Testy integracyjne

```
dotnet test tests/Articles.IntegrationTests/Articles.IntegrationTests.csproj
```

### Wszystkie testy

```
dotnet test
```

## Dokumentacja API

Swagger UI jest dostępny pod adresem:
- http://localhost:5000/swagger

## Przykładowe zapytania (cURL)

### Sprawdzenie stanu API (Health Check)

```
curl -X GET http://localhost:5021/health

Oczekiwana odpowiedź:
```

{
  "status": "Healthy"
}
```

## Struktura projektu

- **src/Articles.API** - Projekt API, zawierający kontrolery i konfigurację
- **src/Articles.Application** - Logika aplikacji, serwisy, handlery komend i zapytań
- **src/Articles.Domain** - Modele domeny, encje, wartości, reguły biznesowe
- **src/Articles.Infrastructure** - Implementacje infrastruktury (baza danych, zewnętrzne API)
- **tests/Articles.UnitTests** - Testy jednostkowe
- **tests/Articles.IntegrationTests** - Testy integracyjne

