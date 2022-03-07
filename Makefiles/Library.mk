.PHONY: restore build test clear-build app-help

restore: ## Restore the project dependencies.
	@echo "[INF] - Restoring the project dependencies...."
	@dotnet restore

build: clean-build restore ## Restore then Build the project.
	@echo "[INF] - Building the project...."
	@dotnet build

test: ## Run the library tests.
	@echo "[INF] - Testing the library..."
	dotnet test

clean-build: ## Clean the project Build.
	@echo "[INF] - Cleanning the project...."
	@find . -not -path "./.git/*" -not -path "./.github/*" | grep -E '(bin|obj)' | xargs rm -rf

app-help: ## Shows the current Makefile Commands.
	@echo "" 
	@echo "==================================== APP ===================================="
	@grep -E '^[a-zA-Z_-]+:.*$$' ./Makefiles/Library.mk | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'
	@echo "============================================================================="