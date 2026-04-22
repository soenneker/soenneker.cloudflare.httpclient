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
    public void Default()
    {

    }
}
