namespace Cassandra.Fluent.Migrator.Example.Controllers
{
    using Core;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/migrator")]
    public class CassandraMigratorController : ControllerBase
    {
        private readonly ICassandraMigrator migrator;

        public CassandraMigratorController(ICassandraMigrator migrator)
        {
            this.migrator = migrator;
        }

        [HttpGet]
        [Route("registered")]
        public IActionResult GetRegisteredMigrationAsync()
        {
            return this.Ok(migrator.GetRegisteredMigrations());
        }

        [HttpGet]
        [Route("applied")]
        public IActionResult GetAppliedMigrationAsync()
        {
            return this.Ok(migrator.GetAppliedMigrations());
        }

        [HttpGet]
        [Route("latest")]
        public IActionResult GetLatestAppliedMigrationAsync()
        {
            return this.Ok(migrator.GetLatestMigration());
        }
    }
}