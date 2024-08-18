using ByteFetch.Shared;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace ByteFetch.Core;

internal class DownloadInfo
{
    private readonly DownloadStatus _downloadStatus;
    public readonly string URL;
    public readonly string FileName;
    public long StreamedSize = 0;
    public long WritedSize = 0;
    public bool IsDownloadComplete = false;
    private HttpContentHeaders? _headers;

    public DownloadInfo(DownloadStatus downloadStatus, string url)
    {
        _downloadStatus = downloadStatus;
        URL = url;
        FileName = Path.GetFileName(URL);
    }

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
        => downloadModel.DownloadSize = (long)_headers.ContentLength!;

}