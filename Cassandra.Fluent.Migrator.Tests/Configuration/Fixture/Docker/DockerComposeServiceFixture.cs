namespace Cassandra.Fluent.Migrator.Tests.Configuration.Fixture.Docker;

using System.Collections.Generic;
using System.IO;
using System.Threading;
using Ductus.FluentDocker.Model.Common;
using Ductus.FluentDocker.Model.Compose;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Impl;

public class DockerComposeServiceFixture : DockerComposeService
{
    private const string SERVICE_NAME = "cfm-test-database";

    protected override ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(),
                (TemplateString)"Configuration/Fixture/Docker/docker-compose.yml");

        return new DockerComposeCompositeService(this.DockerHost,
                new DockerComposeConfig
                {
                    AlternativeServiceName = SERVICE_NAME,
                    Services = new[] { SERVICE_NAME },
                    ComposeFilePath = new List<string> { file },
                    ForceRecreate = true,
                    RemoveOrphans = true,
                    StopOnDispose = true,
                    AlwaysPull = true
                });
    }

    protected override void OnContainerInitialized()
    {
        /*
         * Once the Container is Up and running
         * we should wait for 1min the time to let
         * the Cassandra server to initialize and
         * be ready for connections.
         */
        Thread.Sleep(60 * 1000);
    }
}