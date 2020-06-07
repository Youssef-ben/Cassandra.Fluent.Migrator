.PHONY: start stop restart remove clean cqlsh queries cassandra-help

start: ## Start the container if exists. Otherwise it pulls the image and create a new database container.
	@echo "Building and starting the Cassandra container...."
	@docker-compose up -d

	@tput setaf 2
	@echo "The Cassandra container {cfm-database} is up and running."
	@echo "Contact Point: localhost" 
	@echo "Port : 9042"
	@tput sgr0

stop: ## Stops the cassandra container if it's up.
	@echo "Stopping the Cassandra container..."
	@docker-compose stop

restart: ## Restart the cassandra container.
	@echo "Restarting the cassandra container..."
	@docker-compose restart

remove: stop ## Stops and remove the cassandra container
	@echo "Removing the Cassandra container..."
	@docker-compose down -v

clean: ## Remove the container and its image.
	@echo "Cleaning Cassandra databse..."
	@docker-compose down -v --rmi all

cqlsh: ## Start Cassandra {CQLSH} console to query the database.
	@echo "Starting Cassandra {cqlsh} console..."
	@docker exec -it cfm-database cqlsh

queries: ## Show some queries that can be used for tests.
	@echo "List available keyspaces: DESCRIBE keyspaces;"
	@echo "Start using a keyspace  : USE <keyspace_name>;"
	@echo "Get the list of tables  : DESCRIBE TABLES;"
	@echo "Query a table data      : SELECT * FROM <table_name>;"
	@echo "Check if a table exists : SELECT table_name FROM system_schema.tables WHERE keyspace_name='<keyspace_name>' AND table_name='<table_name>';"

cassandra-help: ## Shows the Current Makefile Commands.
	@echo "" 
	@echo "================================= CASSANDRA ================================="
	@echo "============================================================================="
	@grep -E '^[a-zA-Z_-]+:.*$$' ./Makefiles/Cassandra.mk | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'