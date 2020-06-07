.PHONY: pack publish remove-package release-help

LIBRARY_NAME	= Cassandra.Fluent.Migrator
VERSION 		= undefined
NUGET_KEY		= undefined

pack: remove-package ## Pack the library and generate the output in the {NuGet.Packages}. Require {VERSION}
ifeq ($(VERSION),undefined)
	$(error Version required!)
endif

	@echo "Packing the library {$(LIBRARY_NAME).$(VERSION)}..."

	@dotnet pack $(LIBRARY_NAME)/$(LIBRARY_NAME).csproj -p:PackageVersion=$(VERSION) -o ./NuGet.Packages

	@echo "Package was created under {./Nuget.Packages/$(LIBRARY_NAME).$(VERSION)}"

publish: pack ## Pack and Publish the library to the NuGet repository. Require {VERSION, NUGET_KEY}
ifeq ($(NUGET_KEY),undefined)
	$(error NuGet Key required!)
endif

	@echo "Publishing the package {./Nuget.Packages/$(LIBRARY_NAME).$(VERSION)}"
	@dotnet nuget push ./Nuget.Packages/$(LIBRARY_NAME).$(VERSION).nupkg -s https://api.nuget.org/v3/index.json -k $(NUGET_KEY)

remove-package: ## Delete the specified package version. Require {VERSION}	
ifeq ($(VERSION),undefined)
	$(error Version required!)
endif

	@echo "Removing the package {./Nuget.Packages/$(LIBRARY_NAME).$(VERSION)}"
	@rm -rf ./Nuget.Packages/$(LIBRARY_NAME).$(VERSION).nupkg

release-help: ## Shows the current Makefile Commands.
	@echo "" 
	@echo "==================================== APP ===================================="
	@echo "============================================================================="
	@grep -E '^[a-zA-Z_-]+:.*$$' ./Makefiles/Library.mk | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'