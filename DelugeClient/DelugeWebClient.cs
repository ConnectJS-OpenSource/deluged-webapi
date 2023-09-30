using System.Net.Http.Headers;
using System.Net;
using System.Security.Authentication;
using System.Xml;
using System.Text.Json;

namespace DelugeClient
{
    public enum TorrentAction
    {
        Start,
        Stop,
        Delete,
        DeleteWithData
    }

    public struct NewTorrent
    {
        public string Path { get; set; }
        public string Type { get; set; }
    }

    public class DelugeWebClient
    {
        private HttpClientHandler _httpClientHandler;
        private HttpClient _httpClient;
        private int _RequestId;
        public String Url { get; private set; }
        private string _password;

        public Dictionary<string, string> PathMappings = new Dictionary<string, string>()
        {
            {"movie", "E:\\Movies" },
            {"tv", "E:\\TV" }
        };

        public DelugeWebClient(String url)
        {
            _httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer(),
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            _httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            _httpClient = new HttpClient(_httpClientHandler, true);
            _RequestId = 1;

            Url = url;
        }

        public async Task LoginAsync(string password)
        {
            var result = await SendRequestAsync<Boolean>("auth.login", password);
            if (!result) throw new AuthenticationException("Failed to login.");
        }

        public Task<Boolean> AuthCheckSessionAsync()
        {
            return SendRequestAsync<Boolean>("auth.check_session");
        }

        public async Task LogoutAsync()
        {
            var result = await SendRequestAsync<Boolean>("auth.delete_session");
            if (!result) throw new Exception("Failed to delete session.");
        }

        public Task<String> AddTorrentMagnetAsync(String uri, TorrentOptions options = null)
        {
            if (String.IsNullOrWhiteSpace(uri)) throw new ArgumentException(nameof(uri));
            var req = CreateRequest("core.add_torrent_magnet", uri, options);
            return SendRequestAsync<String>(req);
        }

        public Task<Boolean> RemoveTorrentAsync(String torrentId, Boolean removeData = false)
        {
            return SendRequestAsync<Boolean>("core.remove_torrent", torrentId, removeData);
        }

        public async Task<List<TorrentStatus>> GetTorrentsStatusAsync(params string[] TorrentIds)
        {
            var filterDict = new Dictionary<string, string[]>();
            if (TorrentIds.Length > 0)
                filterDict.Add("hash", TorrentIds);

            var keys = Utils.GetAllJsonPropertyFromType(typeof(TorrentStatus));
            Dictionary<String, TorrentStatus> result = await SendRequestAsync<Dictionary<String, TorrentStatus>>("core.get_torrents_status", filterDict, keys);
            return result.Values.ToList();
        }

        public async Task<SessionStatus> GetSessionStatusAsync()
        {
            var keys = Utils.GetAllJsonPropertyFromType(typeof(SessionStatus));
            var result = await SendRequestAsync<SessionStatus>("core.get_session_status", keys);
            return result;
        }

        public async Task TorrentActionsAsync(string TorrentId, TorrentAction Action)
        {
            switch (Action)
            {
                case TorrentAction.Start:
                    await SendRequestAsync<object>("core.resume_torrent", TorrentId);
                    break;
                case TorrentAction.Stop:
                    await SendRequestAsync<object>("core.pause_torrent", TorrentId);
                    break;
                case TorrentAction.Delete:
                    await SendRequestAsync<object>("core.remove_torrent", TorrentId, false);
                    break;
                case TorrentAction.DeleteWithData:
                    await SendRequestAsync<object>("core.remove_torrent", TorrentId, true);
                    break;
            }
        }

        public async Task<object> ExecuteRaw(string method, params object[] args)
        {
            var req = CreateRequest(method, args);
            return await SendRequestAsync<object>(req);
        }


        public Task<DelugeConfig> GetConfigAsync()
        {
            return SendRequestAsync<DelugeConfig>("core.get_config");
        }

        private Task<T> SendRequestAsync<T>(string method, params object[] parameters)
        {
            return SendRequestAsync<T>(CreateRequest(method, parameters));
        }

        private async Task<T> SendRequestAsync<T>(WebRequestMessage webRequest)
        {
            var requestJson = JsonSerializer.Serialize(webRequest);

            var responseJson = await PostJson(requestJson);
            
            var webResponse = JsonSerializer.Deserialize<WebResponseMessage<T>>(responseJson);

            if (webResponse.Error != null) throw new DelugeWebClientException(webResponse.Error.Message, webResponse.Error.Code);
            if (webResponse.ResponseId != webRequest.RequestId) throw new DelugeWebClientException("Desync.", 0);

            return webResponse.Result;
        }

        private async Task<String> PostJson(String json)
        {
            StringContent content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

            var responseMessage = await _httpClient.PostAsync(Url, content);
            responseMessage.EnsureSuccessStatusCode();

            var responseJson = await responseMessage.Content.ReadAsStringAsync();
            return responseJson;
        }

        private WebRequestMessage CreateRequest(string method, params object[] parameters)
        {
            if (String.IsNullOrWhiteSpace(method)) throw new ArgumentException(nameof(method));
            return new WebRequestMessage(_RequestId++, method, parameters);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_httpClient != null) _httpClient.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DelugeWebClient() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
