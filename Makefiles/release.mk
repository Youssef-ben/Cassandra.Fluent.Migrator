.PHONY: pack publish remove-package release-help

LIBRARY_NAME	= Cassandra.Fluent.Migrator
VERSION 		= $(shell ./version-bump.sh -get ./$(LIBRARY_NAME)/$(LIBRARY_NAME).csproj)
NUGET_KEY		= undefined

# Used for bumping the version. Accepted values [major|minor|patch].
PART			= patch

bump-version: ## Bumps the version of the file. [PART=[major|minor|patch]]
	@echo "[INF] - Bumping the [$(PART)] part of the current version [$(VERSION)]..."
	@$(eval VERSION = $(shell ./version-bump.sh $(PART) ./$(LIBRARY_NAME)/$(LIBRARY_NAME).csproj))
	@echo "[INF] - Bumped the [$(PART)] version to [${VERSION}]!"

pack: bump-version remove-package ## Pack the library and generate the output in the {NuGet.Packages}. Require {VERSION}
ifeq ($(VERSION),undefined)
	$(error Version required!)
endif

	@echo "[INF] - Packing the library {$(LIBRARY_NAME).$(VERSION)}..."

	@dotnet pack $(LIBRARY_NAME)/$(LIBRARY_NAME).csproj -p:PackageVersion=$(VERSION) -o ./NuGet.Packages

	@echo "[INF] - Package was created under {./Nuget.Packages/$(LIBRARY_NAME).$(VERSION)}"

publish: pack ## Pack and Publish the library to the NuGet repository. Require {VERSION, NUGET_KEY}
ifeq ($(NUGET_KEY),undefined)
	$(error NuGet Key required!)
endif

	@echo "[INF] - Publishing the package {./Nuget.Packages/$(LIBRARY_NAME).$(VERSION)}"
	@dotnet nuget push ./Nuget.Packages/$(LIBRARY_NAME).$(VERSION).nupkg -s https://api.nuget.org/v3/index.json -k $(NUGET_KEY)

remove-package: ## Delete the specified package version. Require {VERSION}	
ifeq ($(VERSION),undefined)
	$(error Version required!)
endif

	@echo "[INF] - Removing the package {./Nuget.Packages/$(LIBRARY_NAME).$(VERSION)}"
	@rm -rf ./Nuget.Packages/$(LIBRARY_NAME).$(VERSION).nupkg

release-help: ## Shows the current Makefile Commands.
	@echo "" 
	@echo "==================================== RELEASE ===================================="
	@grep -E '^[a-zA-Z_-]+:.*$$' ./Makefiles/Release.mk | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'
	@echo "============================================================================="