namespace Cassandra.Fluent.Migrator.Tests.Configuration.Fixture.Docker;

using Xunit;

[CollectionDefinition(COLLECTION_NAME)]
public class DockerComposeServiceFixtureCollection : ICollectionFixture<DockerComposeServiceFixture>
{
    public const string COLLECTION_NAME = "Cassandra Docker Container Collection";
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}