using System;
using System.Threading.Tasks;
using System.Threading;

namespace Soenneker.Cloudflare.HttpClient.Abstract;

/// <summary>
/// A .NET thread-safe singleton HttpClient for 
/// </summary>
public interface ICloudflareHttpClient: IDisposable, IAsyncDisposable
{
    ValueTask<System.Net.Http.HttpClient> Get(CancellationToken cancellationToken = default);
}
