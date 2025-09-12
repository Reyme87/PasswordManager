using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Models
{
    class WebParser
    {
        private readonly HttpClient _httpClient;

        public WebParser()
        {
            _httpClient = new HttpClient();

        }

        public async Task<bool> CheckExistanceAsync(string url)
        {
            try
            {
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "https://" + url;
                }

                using var request = new HttpRequestMessage(HttpMethod.Head, url);
                using var response = await _httpClient.SendAsync(request);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string> GetFaviconUriAsync(string url)
        {
            try
            {
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "https://" + url;
                }

                var baseUri = new Uri(url);
                var faviconUrl = new Uri(baseUri, "/favicon.ico").AbsoluteUri;

                var response = await _httpClient.GetAsync(faviconUrl);
                if (response.IsSuccessStatusCode)
                    return faviconUrl;
                return "";
            }
            catch 
            {
                return "";
            }
        }
    }
}
