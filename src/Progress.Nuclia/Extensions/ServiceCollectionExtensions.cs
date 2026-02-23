using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Progress.Nuclia;
using Microsoft.Extensions.Http;

namespace Progress.Nuclia.Extensions;

/// <summary>
/// Extension methods for configuring Progress.Nuclia services in dependency injection container
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds NucliaDbClient to the service collection with the specified configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="config">Progress.Nuclia configuration</param>
    /// <returns>A builder for fluent configuration</returns>
    public static INucliaDbBuilder AddNucliaDb(this IServiceCollection services, NucliaDbConfig config)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        // Register the configuration
        services.AddSingleton(config);

        return new NucliaDbBuilder(services, config);
    }

    /// <summary>
    /// Adds NucliaDbClient to the service collection with configuration from a factory
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configFactory">Factory function to create the configuration</param>
    /// <returns>A builder for fluent configuration</returns>
    public static INucliaDbBuilder AddNucliaDb(this IServiceCollection services, Func<IServiceProvider, NucliaDbConfig> configFactory)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (configFactory == null)
            throw new ArgumentNullException(nameof(configFactory));

        // Register the configuration factory
        services.AddSingleton(configFactory);

        return new NucliaDbBuilder(services, configFactory);
    }

    /// <summary>
    /// Adds a keyed NucliaDbClient to the service collection with the specified configuration.
    /// This allows multiple NucliaDbClient instances with different configurations to coexist.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="serviceKey">The key to identify this NucliaDb client instance</param>
    /// <param name="config">NucliaDb configuration</param>
    /// <returns>A builder for fluent configuration</returns>
    public static IKeyedNucliaDbBuilder AddKeyedNucliaDb(this IServiceCollection services, object serviceKey, NucliaDbConfig config)
	{
		if (services == null)
			throw new ArgumentNullException(nameof(services));
		if (serviceKey == null)
			throw new ArgumentNullException(nameof(serviceKey));
		if (config == null)
			throw new ArgumentNullException(nameof(config));

		// Register the configuration with the key
		services.AddKeyedSingleton(serviceKey, config);

		// Register named HttpClient for this keyed instance
		var clientName = $"NucliaDb_{serviceKey}";
		services.AddHttpClient(clientName);

		return new KeyedNucliaDbBuilder(services, serviceKey, config, clientName);
	}

	/// <summary>
	/// Adds a keyed NucliaDbClient to the service collection with configuration from a factory.
	/// This allows multiple NucliaDbClient instances with different configurations to coexist.
	/// </summary>
	/// <param name="services">The service collection</param>
	/// <param name="serviceKey">The key to identify this NucliaDb client instance</param>
	/// <param name="configFactory">Factory function to create the configuration</param>
	/// <returns>A builder for fluent configuration</returns>
	public static IKeyedNucliaDbBuilder AddKeyedNucliaDb(this IServiceCollection services, object serviceKey, Func<IServiceProvider, NucliaDbConfig> configFactory)
	{
		if (services == null)
			throw new ArgumentNullException(nameof(services));
		if (serviceKey == null)
			throw new ArgumentNullException(nameof(serviceKey));
		if (configFactory == null)
			throw new ArgumentNullException(nameof(configFactory));

		// Register the configuration factory with the key
		services.AddKeyedSingleton(serviceKey, configFactory);

		// Register named HttpClient for this keyed instance
		var clientName = $"NucliaDb_{serviceKey}";
		services.AddHttpClient(clientName);

		return new KeyedNucliaDbBuilder(services, serviceKey, configFactory, clientName);
	}
}

/// <summary>
/// Builder interface for fluent keyed NucliaDb configuration
/// </summary>
public interface IKeyedNucliaDbBuilder
{
	/// <summary>
	/// The service collection being configured
	/// </summary>
	IServiceCollection Services { get; }

	/// <summary>
	/// The key identifying this NucliaDb client instance
	/// </summary>
	object ServiceKey { get; }

	/// <summary>
	/// Enables logging for the keyed NucliaDbClient
	/// </summary>
	/// <returns>The builder for method chaining</returns>
	IKeyedNucliaDbBuilder UseLogging();

	/// <summary>
	/// Configures a custom HttpClient for the keyed NucliaDbClient
	/// </summary>
	/// <param name="httpClientFactory">Factory function to create the HttpClient</param>
	/// <returns>The builder for method chaining</returns>
	IKeyedNucliaDbBuilder UseHttpClient(Func<IServiceProvider, HttpClient> httpClientFactory);
}

/// <summary>
/// Builder interface for fluent Progress.Nuclia configuration
/// </summary>
public interface INucliaDbBuilder
{
    /// <summary>
    /// The service collection being configured
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Enables logging for NucliaDbClient
    /// </summary>
    /// <returns>The builder for method chaining</returns>
    INucliaDbBuilder UseLogging();

    /// <summary>
    /// Configures a custom HttpClient for NucliaDbClient
    /// </summary>
    /// <param name="httpClientFactory">Factory function to create the HttpClient</param>
    /// <returns>The builder for method chaining</returns>
    INucliaDbBuilder UseHttpClient(Func<IServiceProvider, HttpClient> httpClientFactory);
}

/// <summary>
/// Implementation of the Progress.Nuclia builder
/// </summary>
internal class NucliaDbBuilder : INucliaDbBuilder
{
    private readonly NucliaDbConfig? _config;
    private readonly Func<IServiceProvider, NucliaDbConfig>? _configFactory;
    private bool _useLogging = false;
    private Func<IServiceProvider, HttpClient>? _httpClientFactory;

    public IServiceCollection Services { get; }

    public NucliaDbBuilder(IServiceCollection services, NucliaDbConfig config)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        
        // Register the client immediately with default settings
        RegisterNucliaDbClient();
    }

    public NucliaDbBuilder(IServiceCollection services, Func<IServiceProvider, NucliaDbConfig> configFactory)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        _configFactory = configFactory ?? throw new ArgumentNullException(nameof(configFactory));
        
        // Register the client immediately with default settings
        RegisterNucliaDbClient();
    }

    public INucliaDbBuilder UseLogging()
    {
        _useLogging = true;
        RegisterNucliaDbClient();
        return this;
    }

    public INucliaDbBuilder UseHttpClient(Func<IServiceProvider, HttpClient> httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        RegisterNucliaDbClient();
        return this;
    }

    private void RegisterNucliaDbClient()
    {
        // Remove any existing registration
        var existingDescriptor = Services.FirstOrDefault(d => d.ServiceType == typeof(INucliaDbClient));
        if (existingDescriptor != null)
        {
            Services.Remove(existingDescriptor);
        }

        Services.AddSingleton<INucliaDbClient>(serviceProvider =>
        {
            var config = _config ?? _configFactory!(serviceProvider);
            
            ILoggerFactory? loggerFactory = null;
            if (_useLogging)
            {
                loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            }

            if (_httpClientFactory != null)
            {
                var httpClient = _httpClientFactory(serviceProvider);
                return new NucliaDbClient(httpClient, config, loggerFactory);
            }

            return new NucliaDbClient(config, loggerFactory);
        });
    }
}

/// <summary>
/// Implementation of the keyed NucliaDb builder
/// </summary>
internal class KeyedNucliaDbBuilder : IKeyedNucliaDbBuilder
{
	private readonly NucliaDbConfig? _config;
	private readonly Func<IServiceProvider, NucliaDbConfig>? _configFactory;
	private readonly string _httpClientName;
	private bool _useLogging = false;
	private Func<IServiceProvider, HttpClient>? _httpClientFactory;

	public IServiceCollection Services { get; }
	public object ServiceKey { get; }

	public KeyedNucliaDbBuilder(IServiceCollection services, object serviceKey, NucliaDbConfig config, string httpClientName)
	{
		Services = services ?? throw new ArgumentNullException(nameof(services));
		ServiceKey = serviceKey ?? throw new ArgumentNullException(nameof(serviceKey));
		_config = config ?? throw new ArgumentNullException(nameof(config));
		_httpClientName = httpClientName ?? throw new ArgumentNullException(nameof(httpClientName));

		// Register the client immediately with default settings
		RegisterKeyedNucliaDbClient();
	}

	public KeyedNucliaDbBuilder(IServiceCollection services, object serviceKey, Func<IServiceProvider, NucliaDbConfig> configFactory, string httpClientName)
	{
		Services = services ?? throw new ArgumentNullException(nameof(services));
		ServiceKey = serviceKey ?? throw new ArgumentNullException(nameof(serviceKey));
		_configFactory = configFactory ?? throw new ArgumentNullException(nameof(configFactory));
		_httpClientName = httpClientName ?? throw new ArgumentNullException(nameof(httpClientName));

		// Register the client immediately with default settings
		RegisterKeyedNucliaDbClient();
	}

	public IKeyedNucliaDbBuilder UseLogging()
	{
		_useLogging = true;
		RegisterKeyedNucliaDbClient();
		return this;
	}

	public IKeyedNucliaDbBuilder UseHttpClient(Func<IServiceProvider, HttpClient> httpClientFactory)
	{
		_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
		RegisterKeyedNucliaDbClient();
		return this;
	}

	private void RegisterKeyedNucliaDbClient()
	{
		// Remove any existing keyed registration
		var existingDescriptor = Services.FirstOrDefault(d =>
			d.ServiceType == typeof(INucliaDbClient) &&
			d.ServiceKey != null &&
			d.ServiceKey.Equals(ServiceKey));
		if (existingDescriptor != null)
		{
			Services.Remove(existingDescriptor);
		}

		Services.AddKeyedSingleton<INucliaDbClient>(ServiceKey, (serviceProvider, key) =>
		{
			var config = _config ?? _configFactory!(serviceProvider);

			ILoggerFactory? loggerFactory = null;
			if (_useLogging)
			{
				loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
			}

			if (_httpClientFactory != null)
			{
				var httpClient = _httpClientFactory(serviceProvider);
				return new NucliaDbClient(httpClient, config, loggerFactory);
			}

			// Use IHttpClientFactory to get named HttpClient for better connection pooling
			var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
			var namedHttpClient = httpClientFactory.CreateClient(_httpClientName);
			return new NucliaDbClient(namedHttpClient, config, loggerFactory);
		});
	}
}