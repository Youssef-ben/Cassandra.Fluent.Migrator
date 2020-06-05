namespace Cassandra.Fluent.Migrator.Example.Controllers
{
    using Cassandra.Fluent.Migrator.Core.Models;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/migrator")]
    public class CassandraMigratorController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public IActionResult GetLatestMigration()
        {
            return this.Ok(new MigrationHistory());
        }
    }
}