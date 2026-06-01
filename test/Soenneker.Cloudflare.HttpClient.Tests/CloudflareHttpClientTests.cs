using System;
using System.Threading.Tasks;
using Soenneker.Cloudflare.HttpClient.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Cloudflare.HttpClient.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class CloudflareHttpClientTests : HostedUnitTest
{
    private readonly ICloudflareHttpClient _httpclient;

    public CloudflareHttpClientTests(Host host) : base(host)
    {
        _httpclient = Resolve<ICloudflareHttpClient>(true);
    }

    [Test]
    public async ValueTask Default_uses_configured_api_key()
    {
        System.Net.Http.HttpClient client = await _httpclient.Get();

        string? authorization = client.DefaultRequestHeaders.Authorization?.ToString();

        if (authorization != "Bearer dummy-api-key")
            throw new InvalidOperationException($"Unexpected authorization header: {authorization}");
    }

    [Test]
    public async ValueTask Get_with_api_key_returns_keyed_clients()
    {
        System.Net.Http.HttpClient first = await _httpclient.Get("first-api-key");
        System.Net.Http.HttpClient firstAgain = await _httpclient.Get("first-api-key");
        System.Net.Http.HttpClient second = await _httpclient.Get("second-api-key");

        if (!ReferenceEquals(first, firstAgain))
            throw new InvalidOperationException("Expected the same API key to reuse the cached Cloudflare HttpClient.");

        if (ReferenceEquals(first, second))
            throw new InvalidOperationException("Expected different API keys to use different Cloudflare HttpClient instances.");

        string? firstAuthorization = first.DefaultRequestHeaders.Authorization?.ToString();
        string? secondAuthorization = second.DefaultRequestHeaders.Authorization?.ToString();

        if (firstAuthorization != "Bearer first-api-key")
            throw new InvalidOperationException($"Unexpected first authorization header: {firstAuthorization}");

        if (secondAuthorization != "Bearer second-api-key")
            throw new InvalidOperationException($"Unexpected second authorization header: {secondAuthorization}");
    }
}
