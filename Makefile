# BASE FILE, INCLUDE ALL THE FILES THAT EXISTS \
IN THE {Makefiles/} folder.
.PHONY: help

include ./Makefiles/cassandra.mk
include ./Makefiles/library.mk
include ./Makefiles/release.mk

help: cassandra-help app-help release-help ## Shows the current Makefile Commands.
