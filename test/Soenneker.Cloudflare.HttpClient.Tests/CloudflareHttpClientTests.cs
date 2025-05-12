using Soenneker.Cloudflare.HttpClient.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Cloudflare.HttpClient.Tests;

[Collection("Collection")]
public class CloudflareHttpClientTests : FixturedUnitTest
{
    private readonly ICloudflareHttpClient _httpclient;

    public CloudflareHttpClientTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _httpclient = Resolve<ICloudflareHttpClient>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
