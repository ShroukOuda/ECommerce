namespace ECommerce.IntegrationTests;

public static class TestClientExtensions
{
    public static HttpRequestMessage CreateRequest(
        this HttpClient client,
        HttpMethod method,
        string url,
        bool anonymous = false,
        bool customerOnly = false,
        HttpContent? content = null)
    {
        var request = new HttpRequestMessage(method, url);
        if (anonymous) request.Headers.Add("X-Test-Anonymous", "true");
        if (customerOnly) request.Headers.Add("X-Test-Role", "Customer");
        if (content is not null) request.Content = content;
        return request;
    }
}
