using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
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
    private readonly ConcurrentDictionary<string, byte> _clientIds = new();

    private static readonly Uri _prodBaseUrl = new("https://api.cloudflare.com/client/v4/");

    public CloudflareHttpClient(IHttpClientCache httpClientCache, IConfiguration config)
    {
        _httpClientCache = httpClientCache;
        _apiKey = config.GetValueStrict<string>("Cloudflare:ApiKey");
    }

    public ValueTask<System.Net.Http.HttpClient> Get(CancellationToken cancellationToken = default)
    {
        return Get(_apiKey, cancellationToken);
    }

    public ValueTask<System.Net.Http.HttpClient> Get(string apiKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("Cloudflare API key must be provided.", nameof(apiKey));

        string clientId = GetClientId(apiKey);
        _clientIds.TryAdd(clientId, 0);

        // No closure: state passed explicitly + static lambda
        return _httpClientCache.Get(clientId, apiKey, static apiKey => new HttpClientOptions
        {
            BaseAddress = _prodBaseUrl,
            DefaultRequestHeaders = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {apiKey}" },
            }
        }, cancellationToken);
    }

    public void Dispose()
    {
        foreach (string clientId in _clientIds.Keys)
        {
            _httpClientCache.RemoveSync(clientId);
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (string clientId in _clientIds.Keys)
        {
            await _httpClientCache.Remove(clientId);
        }
    }

    private static string GetClientId(string apiKey)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(apiKey));

        return $"{nameof(CloudflareHttpClient)}:{Convert.ToHexString(hash)}";
    }
}
