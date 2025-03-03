# Makefile
.PHONY: build run test clean docker-build docker-run start-local kill-port migrations-add migrations-apply migrations-rollback migrations-list migrations-script docker-clean test-coverage coverage-report coverage

build:
	docker-compose build

run:
	dotnet run --project src/Articles.API

test:
	dotnet test

clean:
	dotnet clean
	rm -rf **/bin **/obj

docker-build:
	docker-compose build

docker-run:
	docker-compose up -d
	@echo "Aplikacja została uruchomiona w kontenerze Docker. http://localhost:5000/index.html"

docker-stop:
	docker-compose down

kill-port:
	-lsof -ti :5000 | xargs kill -9

start-local: kill-port
	@echo "Uruchamianie aplikacji na porcie 5000..."
	@dotnet run --project src/Articles.API --urls "http://localhost:5000"

# Polecenia do zarządzania migracjami

# Dodanie nowej migracji (użycie: make migrations-add name=NazwaMigracji)
migrations-add:
	@if [ -z "$(name)" ]; then \
		echo "Error: Migration name is missing. Please use: make migrations-add name=NazwaMigracji"; \
		exit 1; \
	fi
	@echo "Adding migration $(name)..."
	docker-compose exec api dotnet ef migrations add $(name) --project src/Articles.Infrastructure --startup-project src/Articles.API

# Zastosowanie wszystkich migracji
migrations-apply:
	@echo "Stosowanie wszystkich migracji..."
	docker-compose exec api dotnet ef database update --project src/Articles.Infrastructure --startup-project src/Articles.API

# Zastosowanie migracji do konkretnej wersji (użycie: make migrations-apply-to target=NazwaMigracji)
migrations-apply-to:
	@if [ -z "$(target)" ]; then \
		echo "Error: Target migration name is missing. Please use: make migrations-apply-to target=NazwaMigracji"; \
		exit 1; \
	fi
	@echo "Applying migration up to $(target)..."
	docker-compose exec api dotnet ef database update $(target) --project src/Articles.Infrastructure --startup-project src/Articles.API

# Cofnięcie ostatniej migracji
migrations-rollback:
	@echo "Rolling back the last migration..."
	docker-compose exec api dotnet ef database update 0 --project src/Articles.Infrastructure --startup-project src/Articles.API

# Cofnięcie do konkretnej migracji (użycie: make migrations-rollback-to target=NazwaMigracji)
migrations-rollback-to:
	@if [ -z "$(target)" ]; then \
		echo "Error: Target migration name is missing. Please use: make migrations-rollback-to target=NazwaMigracji"; \
		exit 1; \
	fi
	@echo "Rolling back migrations to $(target)..."
	docker-compose exec api dotnet ef database update $(target) --project src/Articles.Infrastructure --startup-project src/Articles.API

# Wyświetlenie listy wszystkich migracji
migrations-list:
	@echo "Lista wszystkich migracji:"
	docker-compose exec api dotnet ef migrations list --project src/Articles.Infrastructure --startup-project src/Articles.API

# Generowanie skryptu SQL dla migracji (użycie: make migrations-script from=MigracjaA to=MigracjaB)
migrations-script:
	@echo "Generating SQL script for migrations..."
	@if [ -z "$(from)" ] && [ -z "$(to)" ]; then \
		docker-compose exec api dotnet ef migrations script --idempotent --project src/Articles.Infrastructure --startup-project src/Articles.API; \
	elif [ -z "$(from)" ]; then \
		docker-compose exec api dotnet ef migrations script --idempotent --to $(to) --project src/Articles.Infrastructure --startup-project src/Articles.API; \
	elif [ -z "$(to)" ]; then \
		docker-compose exec api dotnet ef migrations script --idempotent --from $(from) --project src/Articles.Infrastructure --startup-project src/Articles.API; \
	else \
		docker-compose exec api dotnet ef migrations script --idempotent --from $(from) --to $(to) --project src/Articles.Infrastructure --startup-project src/Articles.API; \
	fi

# Usunięcie ostatniej migracji (tylko jeśli nie została zastosowana)
migrations-remove:
	@echo "Removing the last migration..."
	docker-compose exec api dotnet ef migrations remove --project src/Articles.Infrastructure --startup-project src/Articles.API

# Resetowanie bazy danych (usunięcie wszystkich tabel i zastosowanie migracji od nowa)
migrations-reset:
	@echo "Resetting the database..."
	docker-compose exec api dotnet ef database drop --force --project src/Articles.Infrastructure --startup-project src/Articles.API
	docker-compose exec api dotnet ef database update --project src/Articles.Infrastructure --startup-project src/Articles.API


docker-clean:
	@echo "Czyszczenie środowiska Docker..."
	docker compose down -v
	docker system prune -f

.PHONY: up
up:
	docker-compose up -d

.PHONY: down
down:
	docker-compose down

.PHONY: logs
logs:
	docker-compose logs -f

.PHONY: init-db
init-db:
	docker-compose exec api dotnet ef database update --project src/Articles.Infrastructure --startup-project src/Articles.API

# Uruchamia testy z włączonym zbieraniem danych pokrycia
test-coverage:
	dotnet test tests/Articles.UnitTests/Articles.UnitTests.csproj --collect:"XPlat Code Coverage"
	dotnet test tests/Articles.IntegrationTests/Articles.IntegrationTests.csproj --collect:"XPlat Code Coverage"

# Generuje raport pokrycia przy użyciu narzędzia ReportGenerator
coverage-report:
	# Generujemy raporty w formatach HTML, HTML Summary oraz Text Summary
	$(HOME)/.dotnet/tools/reportgenerator "-reports:./**/TestResults/*/coverage.cobertura.xml" "-targetdir:./TestResults/CoverageReport" "-reporttypes:Html;HtmlSummary;TextSummary"
	@echo "Raport pokrycia kodu dostępny w: ./TestResults/CoverageReport/index.html"
	@echo "Podsumowanie pokrycia (Text Summary):"
	@cat ./TestResults/CoverageReport/Summary.txt

# Cel łączący uruchomienie testów i generowanie raportu
coverage: test-coverage coverage-report
