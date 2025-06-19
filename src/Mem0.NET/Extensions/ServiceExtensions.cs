using Mem0.NET.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Mem0.NET.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// Adds Mem0Client to the service collection.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Action to configure Mem0ClientOptions</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddMem0Client(this IServiceCollection services,
        Action<Mem0ClientOptions> configureOptions)
    {
        var options = new Mem0ClientOptions();
        configureOptions(options);

        if (string.IsNullOrEmpty(options.BaseUrl))
        {
            throw new ArgumentException("BaseUrl must be provided in Mem0ClientOptions.");
        }

        services.AddScoped<Mem0Client>(_ => 
            new Mem0Client(options.BaseUrl, options.ApiKey, options.HttpClient));

        return services;
    }
}