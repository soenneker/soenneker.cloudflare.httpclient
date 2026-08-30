using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Cloudflare.HttpClient.Abstract;
using Soenneker.Utils.HttpClientCache.Registrar;

namespace Soenneker.Cloudflare.HttpClient.Registrars;

/// <summary>
/// Registers the Cloudflare HTTP client provider.
/// </summary>
public static class CloudflareHttpClientRegistrar
{
    /// <summary>
    /// Adds <see cref="ICloudflareHttpClient"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddCloudflareHttpClientAsSingleton(this IServiceCollection services)
    {
        services.AddHttpClientCacheAsSingleton().TryAddSingleton<ICloudflareHttpClient, CloudflareHttpClient>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="ICloudflareHttpClient"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddCloudflareHttpClientAsScoped(this IServiceCollection services)
    {
        services.AddHttpClientCacheAsSingleton().TryAddScoped<ICloudflareHttpClient, CloudflareHttpClient>();

        return services;
    }
}
