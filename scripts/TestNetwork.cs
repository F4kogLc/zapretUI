internal static class TestNetwork
{
    readonly static HttpClient httpClient = new();

    public static async Task<bool> TestConnection(string url)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Head, url);
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            request.Headers.ConnectionClose = true;
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));
            using var response = await httpClient.SendAsync(request, cts.Token);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            if (!ex.Message.Contains("A task was canceled"))
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
    }
}
