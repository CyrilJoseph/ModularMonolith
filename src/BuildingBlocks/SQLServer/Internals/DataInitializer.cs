﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CompanyName.MyProjectName.BuildingBlocks.SQLServer.Internals;

internal sealed class DataInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataInitializer> _logger;

    public DataInitializer(IServiceProvider serviceProvider, ILogger<DataInitializer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var initializers = scope.ServiceProvider.GetServices<IDataInitializer>();
        foreach (var initializer in initializers)
        {
            try
            {
                _logger.LogInformation($"Running the initializer: {initializer.GetType().Name}...");
                await initializer.InitAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}