[![](https://img.shields.io/nuget/v/soenneker.cloudflare.httpclient.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.cloudflare.httpclient/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cloudflare.httpclient/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.cloudflare.httpclient/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.cloudflare.httpclient.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.cloudflare.httpclient/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cloudflare.httpclient/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.cloudflare.httpclient/actions/workflows/codeql.yml)

# Soenneker.Cloudflare.HttpClient

Provides cached, bearer-authenticated `HttpClient` instances for Cloudflare's v4 API.

## Installation

```bash
dotnet add package Soenneker.Cloudflare.HttpClient
```

## Configuration

```json
{
  "Cloudflare": {
    "ApiKey": "your-api-token",
    "RequestResponseLogging": false
  }
}
```

`Cloudflare:ApiKey` is required. Enable request/response logging only when debug output is appropriate; the authorization header is redacted by the configured logging handler.

## Registration and usage

```csharp
using Soenneker.Cloudflare.HttpClient.Abstract;
using Soenneker.Cloudflare.HttpClient.Registrars;

services.AddCloudflareHttpClientAsSingleton();

public sealed class CloudflareZoneReader(ICloudflareHttpClient clientProvider)
{
    public async Task<string> GetZone(
        string zoneId,
        CancellationToken cancellationToken)
    {
        HttpClient client = await clientProvider.Get(cancellationToken);
        return await client.GetStringAsync($"zones/{zoneId}", cancellationToken);
    }
}
```

`Get(string apiKey)` creates or reuses a separate client for that token. Every distinct token remains cached until the provider is disposed, so do not feed it an unbounded stream of short-lived credentials.

The provider owns all cache entries it creates. Disposing it removes and disposes those clients. Singleton registration is the normal application-wide lifetime; scoped registration creates isolated cache entries owned by each provider scope.
