# Articles API

Projekt API do zarządzania artykułami, implementujący wzorzec Clean Architecture w ASP.NET Core 8.0.

## Wymagania

- .NET 8.0 SDK - do testów i coverage
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

### Przez Docker

```bash
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
make test
```

### Generowanie raportu pokrycia

```bash
make coverage
```

Aktualne pokrycie kodu:
Podsumowanie pokrycia (Text Summary):
Summary
  Generated on: 02.03.2025 - 21:08:25
  Coverage date: 02.03.2025 - 07:07:46 - 02.03.2025 - 21:07:25
  Parser: MultiReport (13x Cobertura)
  Assemblies: 4
  Classes: 58
  Files: 58
  Line coverage: 68%
  Covered lines: 738
  Uncovered lines: 346
  Coverable lines: 1084
  Total lines: 2033
  Branch coverage: 61.2% (60 of 98)
  Covered branches: 60
  Total branches: 98
  Method coverage: 77.4% (148 of 191)
  Full method coverage: 56.5% (108 of 191)
  Covered methods: 148
  Fully covered methods: 108
  Total methods: 191

Articles.API 96.8%
  Articles.API.Controllers.ArticlesController 96.6%
  Articles.API.Controllers.HealthCheckControllern 100%
                                                               
Articles.Application 82%
Articles.Domain 78.6%
Articles.Infrastructure 46.4%
  

## Dokumentacja API

Swagger UI jest dostępny pod adresem:
- http://localhost:5000/swagger


## Migracje
Dodane zostały też automigracje w środowisku lokalnym - ogólne nie przepadam za tym podejściem, bo sporo w tym magii.

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