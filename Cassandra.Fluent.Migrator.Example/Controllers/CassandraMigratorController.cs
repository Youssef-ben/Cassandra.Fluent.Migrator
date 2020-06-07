namespace Cassandra.Fluent.Migrator.Example.Controllers
{
    using Cassandra.Fluent.Migrator.Core;
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
        [Route("registred")]
        public IActionResult GetResistredMigrationAsync()
        {
            return this.Ok(this.migrator.GetRegistredMigrations());
        }

        [HttpGet]
        [Route("applied")]
        public IActionResult GetAppliedMigrationAsync()
        {
            return this.Ok(this.migrator.GetAppliedMigrations());
        }

        [HttpGet]
        [Route("latest")]
        public IActionResult GetLatestAppliedMigrationAsync()
        {
            return this.Ok(this.migrator.GetLatestMigration());
        }
    }
}