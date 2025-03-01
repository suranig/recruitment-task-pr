# Makefile
.PHONY: build run test clean docker-build docker-run start-local

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
	docker-compose up

docker-stop:
	docker-compose down

start-local:
	dotnet run --project src/Articles.API --urls "http://localhost:5000"
