using System.Text.RegularExpressions;
using Sync.Domain;
using Sync.Domain.Services;
using Sync.Infrastructure.Services.Models;

namespace Sync.Infrastructure.Services;

public class CalibreWebService
{
    private readonly HttpClient _httpClient;

    public CalibreWebService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> LoginAsync(User user, CancellationToken? cancellationToken = null)
    {
        var baseUri = new Uri(user.HostUrl);
        var csrfToken = await GetCsrfToken(baseUri);

        var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
        {
             new ("csrf_token", csrfToken),
             new ("username", user.Username),
             new ("password", user.Password)
        });

        var request = new HttpRequestMessage(HttpMethod.Post, new Uri(baseUri, "/login"))
        {
            Content = content,
        };
        request.Headers.Add("origin", baseUri.ToString());
        request.Headers.Add("referer", new Uri(baseUri, "/login").ToString());

        var loginResponse = await _httpClient.SendAsync(request, cancellationToken ?? CancellationToken.None);
        var loginResultContent = await loginResponse.Content.ReadAsStringAsync(cancellationToken ?? CancellationToken.None);
        return !loginResultContent.Contains("Wrong Username or Password");
    }

    private async Task<string> GetCsrfToken(Uri baseUri)
    {
        var loginPageContent = await _httpClient.GetStringAsync(new Uri(baseUri, "/login"));
        var matches = Regex.Match(loginPageContent, "name=\"csrf_token\" value=\"(.*)\">");
        var csrfToken = matches.Groups[1].Value;
        return csrfToken;
    }

    public async Task<IEnumerable<Book>> GetBooksAsync(User user, CancellationToken? cancellationToken = null)
    {
        if (!await LoginAsync(user, cancellationToken))
            throw new ApplicationException("Credentials are not valid anymore");

        var baseUri = new Uri(user.HostUrl);
        var result = await _httpClient.GetStringAsync(new Uri(baseUri, "/ajax/listbooks?limit=100000000000"), cancellationToken ?? CancellationToken.None);

        var results = await _httpClient.GetFromJsonAsync<CalibreFeed>(new Uri(baseUri, "/ajax/listbooks?limit=100000000000"), cancellationToken ?? CancellationToken.None);

        return results?.Rows.Select(r => new Book(CreateHashes(r), r.Id, r.ReadStatus)).ToList() ?? new List<Book>();
    }

    private static readonly string[] _extensions = new[] { "pdf", "epub", "mobi", "azw3", "docx", "rtf", "fb2", "lit", "lrf",
                           "txt", "htmlz", "rtf", "odt", "cbz", "cbr", "prc" };
    private IEnumerable<string> CreateHashes(CalibreFeedRow rawBook)
    {
        var result = new List<string>();
        var authors = rawBook.Authors.Split(new[] { ',', '&' }, StringSplitOptions.TrimEntries);
        foreach (var author in authors)
        {
            result.AddRange(HashGenerator.CreateHash(rawBook.Title, author, _extensions));
        }
        return result;
    }

    public async Task MarkBookAsReadSync(User user, int calibreId, CancellationToken? cancellationToken = null)
    {
        if (!await LoginAsync(user, cancellationToken))
            throw new ApplicationException("Credentials are not valid anymore");

        var baseUri = new Uri(user.HostUrl);
        var csrfToken = await GetCsrfToken(baseUri);

        var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
        {
             new ("pk", calibreId.ToString()),
             new ("value", "True")
        });

        var request = new HttpRequestMessage(HttpMethod.Post, new Uri(baseUri, "/ajax/editbooks/read_status"))
        {
            Content = content,
        };
        request.Headers.Add("X-Csrftoken", csrfToken);
        request.Headers.Add("origin", baseUri.ToString());
        request.Headers.Add("referer", new Uri(baseUri, "/ajax/editbooks/read_status").ToString());

        var response = await _httpClient.SendAsync(request, cancellationToken ?? CancellationToken.None);
        if (!response.IsSuccessStatusCode)
            throw new ApplicationException("Could not mark book as read");
    }
}