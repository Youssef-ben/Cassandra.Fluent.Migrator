namespace Cassandra.Fluent.Migrator.Tests.Configuration.Fixture.Docker;

using System;
using System.Collections.Generic;
using System.Linq;
using Ductus.FluentDocker.Services;

/// <summary>
///     Inspired form https://gsferreira.com/archive/2023/dotnet-integration-testing-with-docker-compose/
/// </summary>
public abstract class DockerComposeService : IDisposable
{
    private ICompositeService compositeService;
    protected IHostService DockerHost;

    protected DockerComposeService()
    {
        EnsureDockerHost();

        compositeService = Build();
        try
        {
            compositeService.Start();
        }
        catch
        {
            compositeService.Dispose();
            throw;
        }

        OnContainerInitialized();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected abstract ICompositeService Build();

    protected virtual void OnContainerInitialized()
    {
    }

    private void EnsureDockerHost()
    {
        if (DockerHost?.State == ServiceRunningState.Running)
        {
            return;
        }

        IList<IHostService> hosts = new Hosts().Discover();
        DockerHost = hosts.FirstOrDefault(x => x.IsNative) ?? hosts.FirstOrDefault(x => x.Name == "default");

        if (null != DockerHost)
        {
            if (DockerHost.State != ServiceRunningState.Running)
            {
                DockerHost.Start();
            }

            return;
        }

        if (hosts.Count > 0)
        {
            DockerHost = hosts.FirstOrDefault();
        }

        if (null != DockerHost)
        {
            return;
        }

        EnsureDockerHost();
    }

    protected virtual void Dispose(bool disposing)
    {
        try
        {
            compositeService.Dispose();
            compositeService = null!;
        }
        catch
        {
            // ignored
        }
    }
}