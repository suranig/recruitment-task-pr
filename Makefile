# Makefile
.PHONY: build run test clean docker-build docker-run start-local kill-port

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
