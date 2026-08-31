using System;
using System.Threading.Tasks;
using System.Threading;

namespace Soenneker.Cloudflare.HttpClient.Abstract;

/// <summary>
/// Provides cached, authenticated HTTP clients for Cloudflare's v4 API.
/// </summary>
public interface ICloudflareHttpClient: IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the client configured with <c>Cloudflare:ApiKey</c>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<System.Net.Http.HttpClient> Get(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a client configured with the supplied API token.
    /// </summary>
    /// <param name="apiKey">The Cloudflare API token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<System.Net.Http.HttpClient> Get(string apiKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes and disposes the cached client for <c>Cloudflare:ApiKey</c>.
    /// </summary>
    ValueTask<bool> Remove(CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes and disposes the cached client for the supplied API token.
    /// </summary>
    ValueTask<bool> Remove(string apiKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronously removes and disposes the cached client for <c>Cloudflare:ApiKey</c>.
    /// </summary>
    bool RemoveSync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronously removes and disposes the cached client for the supplied API token.
    /// </summary>
    bool RemoveSync(string apiKey, CancellationToken cancellationToken = default);
}
