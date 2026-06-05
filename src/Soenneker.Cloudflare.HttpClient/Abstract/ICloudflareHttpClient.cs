using System;
using System.Threading.Tasks;
using System.Threading;

namespace Soenneker.Cloudflare.HttpClient.Abstract;

/// <summary>
/// A .NET thread-safe singleton HttpClient for 
/// </summary>
public interface ICloudflareHttpClient: IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<System.Net.Http.HttpClient> Get(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="apiKey">The API key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<System.Net.Http.HttpClient> Get(string apiKey, CancellationToken cancellationToken = default);
}
