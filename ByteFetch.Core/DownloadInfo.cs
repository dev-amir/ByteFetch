using ByteFetch.Shared;
using System.Net.Http.Headers;

namespace ByteFetch.Core;

internal class DownloadInfo(DownloadStatus downloadStatus, string url)
{
    private readonly DownloadStatus _downloadStatus = downloadStatus;
    public readonly string URL = url;
    public long StreamedSize = 0;
    public long WritedSize = 0;
    public bool IsDownloadComplete = false;
    private HttpContentHeaders? _headers;

    public async Task GetHeaders()
    {
        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Head, URL);
        var response = await RequestHeaders(client, request);
        if (response != null)
            _headers = response.Content.Headers;
        else
            _downloadStatus.IsRequestHeadersFailed = true;
    }

    private async Task<HttpResponseMessage> RequestHeaders(HttpClient client, HttpRequestMessage request)
        => await client.SendAsync(request).ContinueWith<HttpResponseMessage>(task =>
        {
            if (task.IsFaulted || !task.Result.IsSuccessStatusCode)
                return null;
            else
                return task.Result;
        });

    public void ProcessHeaders(DownloadModel downloadModel)
    {
        downloadModel.DownloadSize = (long)_headers.ContentLength!;
        downloadModel.MediaType = _headers.ContentType.MediaType;
    }
}