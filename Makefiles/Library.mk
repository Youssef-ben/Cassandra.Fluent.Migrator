.PHONY: restore build clear-build test package help

restore: ## Restore the project dependencies.
	@echo "Restoring the project dependencies...."
	@dotnet restore

build: clean-build restore ## Restore then Build the project.
	@echo "Building the project...."
	@dotnet build

test: ## Run the library tests.
	@echo "Testing the library..."
	dotnet test

package: build test ## Create NuGet package.
	@echo "Packaging the library..."

clean-build: ## Clean the project Build.
	@echo "cleanning the project...."
	@rm -rf ./Cassandra.Fluent.Migrator/bin/
	@rm -rf ./Cassandra.Fluent.Migrator/obj/
	@rm -rf ./Cassandra.Fluent.Migrator.Tests/bin/
	@rm -rf ./Cassandra.Fluent.Migrator.Tests/obj/

help-app: ## Shows the current Makefile Commands.
	@echo "" 
	@echo "==================================== APP ===================================="
	@echo "============================================================================="
	@grep -E '^[a-zA-Z_-]+:.*$$' ./Makefiles/Library.mk | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'