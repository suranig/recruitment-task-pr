# Makefile
.PHONY: build run test clean docker-build docker-run start-local kill-port migrations-add migrations-apply migrations-rollback migrations-list migrations-script docker-first-run

build:
	dotnet build

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
		echo "Błąd: Brak nazwy migracji. Użyj: make migrations-add name=NazwaMigracji"; \
		exit 1; \
	fi
	@echo "Dodawanie migracji $(name)..."
	docker-compose exec api dotnet ef migrations add $(name) --project src/Articles.Infrastructure --startup-project src/Articles.API

# Zastosowanie wszystkich migracji
migrations-apply:
	@echo "Stosowanie wszystkich migracji..."
	docker-compose exec api dotnet ef database update --project src/Articles.Infrastructure --startup-project src/Articles.API

# Zastosowanie migracji do konkretnej wersji (użycie: make migrations-apply-to target=NazwaMigracji)
migrations-apply-to:
	@if [ -z "$(target)" ]; then \
		echo "Błąd: Brak nazwy docelowej migracji. Użyj: make migrations-apply-to target=NazwaMigracji"; \
		exit 1; \
	fi
	@echo "Stosowanie migracji do $(target)..."
	docker-compose exec api dotnet ef database update $(target) --project src/Articles.Infrastructure --startup-project src/Articles.API

# Cofnięcie ostatniej migracji
migrations-rollback:
	@echo "Cofanie ostatniej migracji..."
	docker-compose exec api dotnet ef database update 0 --project src/Articles.Infrastructure --startup-project src/Articles.API

# Cofnięcie do konkretnej migracji (użycie: make migrations-rollback-to target=NazwaMigracji)
migrations-rollback-to:
	@if [ -z "$(target)" ]; then \
		echo "Błąd: Brak nazwy docelowej migracji. Użyj: make migrations-rollback-to target=NazwaMigracji"; \
		exit 1; \
	fi
	@echo "Cofanie migracji do $(target)..."
	docker-compose exec api dotnet ef database update $(target) --project src/Articles.Infrastructure --startup-project src/Articles.API

# Wyświetlenie listy wszystkich migracji
migrations-list:
	@echo "Lista wszystkich migracji:"
	docker-compose exec api dotnet ef migrations list --project src/Articles.Infrastructure --startup-project src/Articles.API

# Generowanie skryptu SQL dla migracji (użycie: make migrations-script from=MigracjaA to=MigracjaB)
migrations-script:
	@echo "Generowanie skryptu SQL dla migracji..."
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
	@echo "Usuwanie ostatniej migracji..."
	docker-compose exec api dotnet ef migrations remove --project src/Articles.Infrastructure --startup-project src/Articles.API

# Resetowanie bazy danych (usunięcie wszystkich tabel i zastosowanie migracji od nowa)
migrations-reset:
	@echo "Resetowanie bazy danych..."
	docker-compose exec api dotnet ef database drop --force --project src/Articles.Infrastructure --startup-project src/Articles.API
	docker-compose exec api dotnet ef database update --project src/Articles.Infrastructure --startup-project src/Articles.API

# Pierwsze uruchomienie za pomocą skryptWu
docker-first-run:
	@chmod +x scripts/init-db.sh
	@./scripts/init-db.sh
