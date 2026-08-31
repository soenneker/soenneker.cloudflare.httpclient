using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Soenneker.Extensions.Configuration;
using Soenneker.Cloudflare.HttpClient.Abstract;
using Soenneker.Dtos.HttpClientOptions;
using Soenneker.HttpClients.LoggingHandler;
using Soenneker.Utils.HttpClientCache.Abstract;

namespace Soenneker.Cloudflare.HttpClient;

public sealed class CloudflareHttpClient : ICloudflareHttpClient
{
    private readonly IHttpClientCache _httpClientCache;
    private readonly string _apiKey;
    private readonly bool _requestResponseLogging;
    private readonly ILogger<CloudflareHttpClient> _logger;
    private readonly ConcurrentDictionary<string, byte> _clientIds = new();
    private readonly string _instanceId = Guid.NewGuid().ToString("N");

    private static readonly Uri _prodBaseUrl = new("https://api.cloudflare.com/client/v4/");

    public CloudflareHttpClient(IHttpClientCache httpClientCache, IConfiguration config,
        ILogger<CloudflareHttpClient> logger)
    {
        _httpClientCache = httpClientCache;
        _apiKey = config.GetValueStrict<string>("Cloudflare:ApiKey");
        _requestResponseLogging = config.GetValue<bool>("Cloudflare:RequestResponseLogging");
        _logger = logger;
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
        return _httpClientCache.Get(clientId, (apiKey, logging: _requestResponseLogging, logger: _logger),
            static state => new HttpClientOptions
            {
                BaseAddress = _prodBaseUrl,
                DefaultRequestHeaders = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {state.apiKey}" },
                },
                DelegatingHandlerFactories = state.logging
                    ?
                    [
                        () => new HttpClientLoggingHandler(state.logger, new HttpClientLoggingOptions
                        {
                            LogLevel = LogLevel.Debug,
                            RedactedHeaders = ["Authorization"]
                        })
                    ]
                    : null
            }, cancellationToken);
    }

    public ValueTask<bool> Remove(CancellationToken cancellationToken = default) => Remove(_apiKey, cancellationToken);

    public async ValueTask<bool> Remove(string apiKey, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string clientId = GetClientId(apiKey);
        await _httpClientCache.Remove(clientId);
        return _clientIds.TryRemove(clientId, out _);
    }

    public bool RemoveSync(CancellationToken cancellationToken = default) => RemoveSync(_apiKey, cancellationToken);

    public bool RemoveSync(string apiKey, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string clientId = GetClientId(apiKey);
        _httpClientCache.RemoveSync(clientId);
        return _clientIds.TryRemove(clientId, out _);
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

    private string GetClientId(string apiKey)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(apiKey));

        return $"{nameof(CloudflareHttpClient)}:{_instanceId}:{Convert.ToHexString(hash)}";
    }
}
