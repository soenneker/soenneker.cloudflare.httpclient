using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Soenneker.Extensions.Configuration;
using Soenneker.Cloudflare.HttpClient.Abstract;
using Soenneker.Dtos.HttpClientOptions;
using Soenneker.Utils.HttpClientCache.Abstract;

namespace Soenneker.Cloudflare.HttpClient;

///<inheritdoc cref="ICloudflareHttpClient"/>
public sealed class CloudflareHttpClient : ICloudflareHttpClient
{
    private readonly IHttpClientCache _httpClientCache;
    private readonly string _apiKey;

    private const string _prodBaseUrl = "https://api.cloudflare.com/client/v4/";

    public CloudflareHttpClient(IHttpClientCache httpClientCache, IConfiguration config)
    {
        _httpClientCache = httpClientCache;
        _apiKey = config.GetValueStrict<string>("Cloudflare:ApiKey");
    }

    public ValueTask<System.Net.Http.HttpClient> Get(CancellationToken cancellationToken = default)
    {
        return _httpClientCache.Get(nameof(CloudflareHttpClient), CreateHttpClientOptions, cancellationToken);
    }

    private HttpClientOptions? CreateHttpClientOptions()
    {
        return new HttpClientOptions
        {
            BaseAddress = _prodBaseUrl,
            DefaultRequestHeaders = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {_apiKey}" },
            }
        };
    }

    public void Dispose()
    {
        _httpClientCache.RemoveSync(nameof(CloudflareHttpClient));
    }

    public ValueTask DisposeAsync()
    {
        return _httpClientCache.Remove(nameof(CloudflareHttpClient));
    }
}