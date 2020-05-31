.PHONY: start-cassandra stop-cassandra remove-cassandra restart-cassandra clean start-cqlsh queries-example help-cassandra

## Defining the global values.

DB_PORT				= 9042

DB_IMAGE			= cassandra:latest
DB_CONTAINER_NAME	= cfm-database
DB_CLUSTER_NAME		= cfm-cluster

DB_VOLUME			= cfm-database
DB_NETWORK			= cfm-database

## Check common values.
DB_CONTAINER_EXISTS	= $(shell docker ps -a --no-trunc --quiet --filter name='^$(DB_CONTAINER_NAME)$$' | wc -l | sed -e 's/^[ \t]*//')
DB_CONTAINER_UP		= $(shell docker ps --no-trunc --quiet --filter name='^$(DB_CONTAINER_NAME)$$' | wc -l | sed -e 's/^[ \t]*//')
DB_VOLUME_EXISTS	= $(shell docker volume ls --quiet --filter name='^$(DB_VOLUME)$$' | wc -l | sed -e 's/^[ \t]*//')

DB_NETWORK_EXISTS	= $(shell docker netwok ls --quiet --filter name='^$(DB_NETWORK)$$' | wc -l | sed -e 's/^[ \t]*//')

start-cassandra: ## Start the container if exists. Otherwise it pulls the image and create a new database container.

## Create the cassandra volume if it doesn't exits.
ifeq ($(DB_VOLUME_EXISTS), 0)
	@echo "Creating database volume {$(DB_VOLUME)}..."
	@docker volume create $(DB_VOLUME) > /dev/null;
endif

## Create the cassandra network if it doesn't exits.
ifeq ($(DB_VOLUME_EXISTS), 0)
	@echo "Creating database network {$(DB_NETWORK)}..."
	@docker network create -d bridge  $(DB_NETWORK) > /dev/null;
endif

ifeq ($(DB_CONTAINER_EXISTS), 0)
	@echo "Creating a new database container {$(DB_CONTAINER_NAME)}..."	
	@docker run \
		--name $(DB_CONTAINER_NAME) \
		--network $(DB_NETWORK) \
		-e CASSANDRA_CLUSTER_NAME=$(DB_CLUSTER_NAME) \
		-p $(DB_PORT):9042 \
		-v $(DB_VOLUME):/var/lib/cassandra \
		-d $(DB_IMAGE) > /dev/null


else ifeq ($(DB_CONTAINER_UP), 0)
	@echo "Starting the database container {$(DB_CONTAINER_NAME)}..."
	@docker start $(DB_CONTAINER_NAME) > /dev/null
endif

	@tput setaf 2
	@echo "The Cassandra container {$(DB_CONTAINER_NAME)} is up and running."
	@echo "Contact Point: localhost" 
	@echo "Port : $(DB_PORT)"
	@tput sgr0

stop-cassandra: ## Stops the cassandra container if it's up.
ifeq ($(DB_CONTAINER_UP), 1)
	@echo "Stopping the Cassandra container $(DB_CONTAINER_NAME)..."
	@docker stop $(DB_CONTAINER_NAME) > /dev/null
endif

remove-cassandra: stop-cassandra ## Stops and remove the cassandra container
ifeq ($(DB_CONTAINER_EXISTS), 1)
	@echo "Removing the Cassandra container $(DB_CONTAINER_NAME)..."
	@docker rm $(DB_CONTAINER_NAME) > /dev/null
endif

restart-cassandra: stop-cassandra start-cassandra ## Restart the cassandra container.

start-cqlsh: ## Start Cassandra {CQLSH} console to query the database.
	@echo "Starting Cassandra {cqlsh} console..."
	@docker exec -it $(DB_CONTAINER_NAME) cqlsh

clean: remove-cassandra ## Remove the container and delete the volume and image.
	@echo "Deleting the volume $(DB_VOLUME)..."
	@docker volume rm $(DB_VOLUME) > /dev/null

	@echo "Deleting the cassandra image $(DB_IMAGE)..."
	@docker rmi $(DB_IMAGE)  > /dev/null

queries-example: ## Show some queries that can be used for tests.
	@echo "List available keyspaces: DESCRIBE keyspaces;"
	@echo "Start using a keyspace  : USE <keyspace_name>;"
	@echo "Get the list of tables  : DESCRIBE TABLES;"
	@echo "Query a table data      : SELECT * FROM <table_name>;"
	@echo "Check if a table exists : SELECT table_name FROM system_schema.tables WHERE keyspace_name='<keyspace_name>' AND table_name='<table_name>';"

help-cassandra: ## Shows the Current Makefile Commands.
	@echo "" 
	@echo "================================= CASSANDRA ================================="
	@echo "============================================================================="
	@grep -E '^[a-zA-Z_-]+:.*$$' ./Makefiles/Cassandra.mk | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'