#!/bin/bash

echo "Uruchamianie kontenerów..."
docker-compose up -d --build

echo "Czekam na uruchomienie bazy danych..."
until docker-compose exec -T db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P LocalDB123@ -Q "SELECT 1" > /dev/null 2>&1
do
    echo "Baza danych jeszcze nie jest gotowa - czekam..."
    sleep 2
done

echo "Baza danych jest gotowa! Stosowanie migracji..."
docker-compose exec -T api dotnet ef database update --project src/Articles.Infrastructure --startup-project src/Articles.API

echo "Aplikacja jest gotowa! Dostępna pod adresem http://localhost:5000" 