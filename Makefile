.PHONY: restore build clear-build help

restore: ## Restore the project dependencies.
	@echo "Restoring the project dependencies...."
	@dotnet restore

build: restore ## Restore then Build the project.
	@echo "Building the project...."
	@dotnet build

clean-build: ## Clean the project Build.
	@echo "cleanning the project...."
	@rm -rf ./Cassandra.Fluent.Migrator/bin/
	@rm -rf ./Cassandra.Fluent.Migrator/obj/

help: ## Shows the current Makefile Commands.
	@grep -E '^[a-zA-Z_-]+:.*$$' ./Makefile | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'