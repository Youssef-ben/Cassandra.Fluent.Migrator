# BASE FILE, INCLUDE ALL THE FILES THAT EXISTS \
IN THE {Makefiles/} folder.
.PHONY: help

include ./Makefiles/Cassandra.mk
include ./Makefiles/Library.mk

help: help-cassandra help-app ## Shows the current Makefile Commands.
	@echo "" 
	@echo "==================================== BASE ==================================="
	@echo "============================================================================="
	@grep -E '^[a-zA-Z_-]+:.*$$' ./Makefile | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'