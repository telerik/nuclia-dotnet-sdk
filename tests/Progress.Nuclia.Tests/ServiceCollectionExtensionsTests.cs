using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Progress.Nuclia;
using Progress.Nuclia.Extensions;
using System;
using System.Net.Http;
using Xunit;

namespace Progress.Nuclia.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddNucliaDb_WithConfig_RegistersServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
        var config = new NucliaDbConfig(
            ZoneId: "test-zone",
            KnowledgeBoxId: "test-kb-id",
            ApiKey: "test-api-key"
        );

        // Act
        services.AddNucliaDb(config).UseLogging();
        
        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        // Verify that NucliaDbConfig is registered
        var registeredConfig = serviceProvider.GetRequiredService<NucliaDbConfig>();
        Assert.NotNull(registeredConfig);
        Assert.Equal("test-zone", registeredConfig.ZoneId);
        Assert.Equal("test-kb-id", registeredConfig.KnowledgeBoxId);
        Assert.Equal("test-api-key", registeredConfig.ApiKey);
        
        // Verify that NucliaDbClient is registered
        var client = serviceProvider.GetRequiredService<INucliaDbClient>();
        Assert.NotNull(client);
        Assert.Equal(config, client.Config);
    }

    [Fact]
    public void AddNucliaDb_WithConfigFactory_RegistersServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
        // Act
        services.AddNucliaDb(sp => new NucliaDbConfig(
            ZoneId: "factory-zone",
            KnowledgeBoxId: "factory-kb-id", 
            ApiKey: "factory-api-key"
        )).UseLogging();
        
        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        // Verify that NucliaDbClient is registered and config factory works
        var client = serviceProvider.GetRequiredService<INucliaDbClient>();
        Assert.NotNull(client);
        Assert.Equal("factory-zone", client.Config.ZoneId);
        Assert.Equal("factory-kb-id", client.Config.KnowledgeBoxId);
        Assert.Equal("factory-api-key", client.Config.ApiKey);
    }

    [Fact]
    public void AddNucliaDb_WithoutLogging_RegistersServices()
    {
        // Arrange
        var services = new ServiceCollection();
        
        var config = new NucliaDbConfig(
            ZoneId: "test-zone",
            KnowledgeBoxId: "test-kb-id",
            ApiKey: "test-api-key"
        );

        // Act
        services.AddNucliaDb(config);
        
        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        // Verify that NucliaDbClient is registered even without explicit UseLogging call
        var client = serviceProvider.GetRequiredService<INucliaDbClient>();
        Assert.NotNull(client);
        Assert.Equal(config, client.Config);
    }

    [Fact]
    public void AddNucliaDb_WithCustomHttpClient_RegistersServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
        var config = new NucliaDbConfig(
            ZoneId: "test-zone",
            KnowledgeBoxId: "test-kb-id",
            ApiKey: "test-api-key"
        );

        var customTimeout = TimeSpan.FromMinutes(10);

        // Act
        services.AddNucliaDb(config)
            .UseHttpClient(sp => 
            {
                var httpClient = new HttpClient();
                httpClient.Timeout = customTimeout;
                return httpClient;
            })
            .UseLogging();
        
        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        // Verify that NucliaDbClient is registered
        var client = serviceProvider.GetRequiredService<INucliaDbClient>();
        Assert.NotNull(client);
        Assert.Equal(config, client.Config);
    }

    [Fact]
    public void AddNucliaDb_SingletonLifetime_ReturnsSameInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
        var config = new NucliaDbConfig(
            ZoneId: "test-zone",
            KnowledgeBoxId: "test-kb-id",
            ApiKey: "test-api-key"
        );

        services.AddNucliaDb(config).UseLogging();
        
        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        var client1 = serviceProvider.GetRequiredService<INucliaDbClient>();
        var client2 = serviceProvider.GetRequiredService<INucliaDbClient>();
        
        // Should be the same instance (singleton lifetime)
        Assert.Same(client1, client2);
    }

	[Fact]
	public void AddKeyedNucliaDb_WithConfig_RegistersKeyedServices()
	{
		// Arrange
		var services = new ServiceCollection();
		services.AddLogging();

		var config = new NucliaDbConfig(
			ZoneId: "test-zone",
			KnowledgeBoxId: "test-kb-id",
			ApiKey: "test-api-key"
		);

		// Act
		services.AddKeyedNucliaDb("primary", config).UseLogging();

		// Assert
		var serviceProvider = services.BuildServiceProvider();

		// Verify that keyed NucliaDbConfig is registered
		var registeredConfig = serviceProvider.GetRequiredKeyedService<NucliaDbConfig>("primary");
		Assert.NotNull(registeredConfig);
		Assert.Equal("test-zone", registeredConfig.ZoneId);
		Assert.Equal("test-kb-id", registeredConfig.KnowledgeBoxId);
		Assert.Equal("test-api-key", registeredConfig.ApiKey);

		// Verify that keyed NucliaDbClient is registered
		var client = serviceProvider.GetRequiredKeyedService<INucliaDbClient>("primary");
		Assert.NotNull(client);
		Assert.Equal(config, client.Config);
	}

	[Fact]
	public void AddKeyedNucliaDb_WithConfigFactory_RegistersKeyedServices()
	{
		// Arrange
		var services = new ServiceCollection();
		services.AddLogging();

		// Act
		services.AddKeyedNucliaDb("secondary", sp => new NucliaDbConfig(
			ZoneId: "factory-zone",
			KnowledgeBoxId: "factory-kb-id",
			ApiKey: "factory-api-key"
		)).UseLogging();

		// Assert
		var serviceProvider = services.BuildServiceProvider();

		// Verify that keyed NucliaDbClient is registered and config factory works
		var client = serviceProvider.GetRequiredKeyedService<INucliaDbClient>("secondary");
		Assert.NotNull(client);
		Assert.Equal("factory-zone", client.Config.ZoneId);
		Assert.Equal("factory-kb-id", client.Config.KnowledgeBoxId);
		Assert.Equal("factory-api-key", client.Config.ApiKey);
	}

	[Fact]
	public void AddKeyedNucliaDb_MultipleInstances_RegistersSeparateServices()
	{
		// Arrange
		var services = new ServiceCollection();
		services.AddLogging();

		var config1 = new NucliaDbConfig(
			ZoneId: "zone-1",
			KnowledgeBoxId: "kb-id-1",
			ApiKey: "api-key-1"
		);

		var config2 = new NucliaDbConfig(
			ZoneId: "zone-2",
			KnowledgeBoxId: "kb-id-2",
			ApiKey: "api-key-2"
		);

		var config3 = new NucliaDbConfig(
			ZoneId: "zone-3",
			KnowledgeBoxId: "kb-id-3",
			ApiKey: "api-key-3"
		);

		// Act - Register multiple keyed clients
		services.AddKeyedNucliaDb("tenant1", config1).UseLogging();
		services.AddKeyedNucliaDb("tenant2", config2).UseLogging();
		services.AddKeyedNucliaDb("tenant3", config3);

		// Assert
		var serviceProvider = services.BuildServiceProvider();

		// Verify each keyed client has its own configuration
		var client1 = serviceProvider.GetRequiredKeyedService<INucliaDbClient>("tenant1");
		var client2 = serviceProvider.GetRequiredKeyedService<INucliaDbClient>("tenant2");
		var client3 = serviceProvider.GetRequiredKeyedService<INucliaDbClient>("tenant3");

		Assert.NotNull(client1);
		Assert.NotNull(client2);
		Assert.NotNull(client3);

		// Each client should have different configurations
		Assert.NotSame(client1, client2);
		Assert.NotSame(client2, client3);
		Assert.NotSame(client1, client3);

		Assert.Equal("zone-1", client1.Config.ZoneId);
		Assert.Equal("kb-id-1", client1.Config.KnowledgeBoxId);

		Assert.Equal("zone-2", client2.Config.ZoneId);
		Assert.Equal("kb-id-2", client2.Config.KnowledgeBoxId);

		Assert.Equal("zone-3", client3.Config.ZoneId);
		Assert.Equal("kb-id-3", client3.Config.KnowledgeBoxId);
	}

	[Fact]
	public void AddKeyedNucliaDb_SingletonLifetime_ReturnsSameInstanceForSameKey()
	{
		// Arrange
		var services = new ServiceCollection();
		services.AddLogging();

		var config = new NucliaDbConfig(
			ZoneId: "test-zone",
			KnowledgeBoxId: "test-kb-id",
			ApiKey: "test-api-key"
		);

		services.AddKeyedNucliaDb("primary", config).UseLogging();

		// Assert
		var serviceProvider = services.BuildServiceProvider();

		var client1 = serviceProvider.GetRequiredKeyedService<INucliaDbClient>("primary");
		var client2 = serviceProvider.GetRequiredKeyedService<INucliaDbClient>("primary");

		// Should be the same instance (singleton lifetime per key)
		Assert.Same(client1, client2);
	}

	[Fact]
	public void AddKeyedNucliaDb_WithCustomHttpClient_RegistersKeyedServices()
	{
		// Arrange
		var services = new ServiceCollection();
		services.AddLogging();

		var config = new NucliaDbConfig(
			ZoneId: "test-zone",
			KnowledgeBoxId: "test-kb-id",
			ApiKey: "test-api-key"
		);

		var customTimeout = TimeSpan.FromMinutes(15);

		// Act
		services.AddKeyedNucliaDb("custom", config)
			.UseHttpClient(sp =>
			{
				var httpClient = new HttpClient();
				httpClient.Timeout = customTimeout;
				return httpClient;
			})
			.UseLogging();

		// Assert
		var serviceProvider = services.BuildServiceProvider();

		// Verify that keyed NucliaDbClient is registered
		var client = serviceProvider.GetRequiredKeyedService<INucliaDbClient>("custom");
		Assert.NotNull(client);
		Assert.Equal(config, client.Config);
	}

	[Fact]
	public void AddKeyedNucliaDb_CoexistsWithNonKeyedRegistration()
	{
		// Arrange
		var services = new ServiceCollection();
		services.AddLogging();

		var defaultConfig = new NucliaDbConfig(
			ZoneId: "default-zone",
			KnowledgeBoxId: "default-kb",
			ApiKey: "default-key"
		);

		var keyedConfig = new NucliaDbConfig(
			ZoneId: "keyed-zone",
			KnowledgeBoxId: "keyed-kb",
			ApiKey: "keyed-key"
		);

		// Act - Register both non-keyed and keyed clients
		services.AddNucliaDb(defaultConfig).UseLogging();
		services.AddKeyedNucliaDb("alternative", keyedConfig).UseLogging();

		// Assert
		var serviceProvider = services.BuildServiceProvider();

		// Verify both can be resolved
		var defaultClient = serviceProvider.GetRequiredService<INucliaDbClient>();
		var keyedClient = serviceProvider.GetRequiredKeyedService<INucliaDbClient>("alternative");

		Assert.NotNull(defaultClient);
		Assert.NotNull(keyedClient);

		// They should be different instances
		Assert.NotSame(defaultClient, keyedClient);

		// With different configurations
		Assert.Equal("default-zone", defaultClient.Config.ZoneId);
		Assert.Equal("keyed-zone", keyedClient.Config.ZoneId);
	}
}