using System.Text.RegularExpressions;
using Newtonsoft.Json;

internal class VersionChecker
{
    static readonly HttpClient httpClient = new()
    {
        DefaultRequestHeaders =
        {
            { "User-Agent", "Zapret-UI" }
        }
    };

    public static async Task<Version> GetLatestGitHubVersion()
    {
        try
        {
            var response = await httpClient.GetStringAsync("https://api.github.com/repos/F4kogLc/zapretUI/releases/latest");

            var releaseData = JsonConvert.DeserializeAnonymousType(response, new { tag_name = "" });

            if (string.IsNullOrEmpty(releaseData?.tag_name))
                return null;

            var cleanVersion = Regex.Replace(releaseData.tag_name, @"^v|/.*$", "", RegexOptions.IgnoreCase);

            return Version.Parse(cleanVersion);
        }
        catch
        {
            return null;
        }
    }

    public static async Task<bool> IsUpdateAvailable()
    {
        var latestVersion = await GetLatestGitHubVersion();
        return latestVersion != null && latestVersion > Consts.VERSION;
    }
}
