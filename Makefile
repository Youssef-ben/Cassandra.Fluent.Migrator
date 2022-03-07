# BASE FILE, INCLUDE ALL THE FILES THAT EXISTS \
IN THE {Makefiles/} folder.
.PHONY: help

include ./Makefiles/Cassandra.mk
include ./Makefiles/Library.mk
include ./Makefiles/Release.mk

help: cassandra-help app-help release-help ## Shows the current Makefile Commands.
