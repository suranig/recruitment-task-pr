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
```bash
dotnet run --project src/Articles.API/Articles.API.csproj
# lub
make start-local
```

API będzie dostępne pod adresem:
- HTTP: http://localhost:5000

### Przez Docker (opcjonalnie)

```bash
# przy pierwszym uruchomieniu
make docker-first-run # uruchamia kontenery i stosuje migracje
# lub
make docker-build && make docker-run
```

## Uruchamianie testów

### Testy jednostkowe

```bash
dotnet test tests/Articles.UnitTests/Articles.UnitTests.csproj
```

### Testy integracyjne

```bash
dotnet test tests/Articles.IntegrationTests/Articles.IntegrationTests.csproj
```

### Wszystkie testy

```bash
dotnet test
```

## Dokumentacja API

Swagger UI jest dostępny pod adresem:
- http://localhost:5000/swagger

## Przykładowe zapytania (cURL)

### Sprawdzenie stanu API (Health Check)

```
curl -X GET http://localhost:5000health

Oczekiwana odpowiedź:
```

{
  "status": "Healthy"
}
```

## Migracje

1. **Dodanie nowej migracji po zmianie modelu**:
   ```bash
   make migrations-add name=NazwaMigracji
   ```

2. **Zastosowanie migracji w środowisku deweloperskim**:
   ```bash
   make migrations-apply
   ```

3. **Cofnięcie problematycznej migracji**:
   ```bash
   make migrations-rollback-to target=PoprzedniaMigracja
   ```

4. **Generowanie skryptu SQL dla środowiska produkcyjnego**:
   ```bash
   make migrations-script > migration.sql
   ```