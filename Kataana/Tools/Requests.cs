using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kataana.Tools
{
    public class Requests
    {
        private HttpClient httpClient { get; set; }

        public Requests()
        {
            httpClient = new HttpClient();
        }

        public async Task<string> Get(string url, string headersJson = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage response = null;

            if (string.IsNullOrEmpty(url) == false)
            {
                SetHeaders(request, headersJson);
                response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode == true)
                {
                    return (await response.Content.ReadAsStringAsync());
                }
            }
            
            return (null);
        }

        public async Task<string> Get(string url, Object headersJson = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage response = null;

            if (string.IsNullOrEmpty(url) == false)
            {
                SetHeaders(request, headersJson);
                response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode == true)
                {
                    return (await response.Content.ReadAsStringAsync());
                }
            }

            return (null);
        }

        public async Task<string> Post(string url, string content, string contentType = "application/json", string headersJson = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            HttpResponseMessage response = null;

            request.Content = new StringContent(content, Encoding.UTF8, contentType);
            if (string.IsNullOrEmpty(url) == false)
            {
                SetHeaders(request, headersJson);
                response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode == true)
                {
                    return (await response.Content.ReadAsStringAsync());
                }
            }

            return (null);
        }

        public async Task<string> Post(string url, string content, string contentType = "application/json", Object headersJson = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            HttpResponseMessage response = null;

            request.Content = new StringContent(content, Encoding.UTF8, contentType);
            if (string.IsNullOrEmpty(url) == false)
            {
                SetHeaders(request, headersJson);
                response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode == true)
                {
                    return (await response.Content.ReadAsStringAsync());
                }
            }

            return (null);
        }

        public async Task<string> Put(string url, string content, string contentType = "application/json", string headersJson = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, url);
            HttpResponseMessage response = null;

            request.Content = new StringContent(content, Encoding.UTF8, contentType);
            if (string.IsNullOrEmpty(url) == false)
            {
                SetHeaders(request, headersJson);
                response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode == true)
                {
                    return (await response.Content.ReadAsStringAsync());
                }
            }

            return (null);
        }

        public async Task<string> Put(string url, string content, string contentType = "application/json", Object headersJson = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, url);
            HttpResponseMessage response = null;

            request.Content = new StringContent(content, Encoding.UTF8, contentType);
            if (string.IsNullOrEmpty(url) == false)
            {
                SetHeaders(request, headersJson);
                response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode == true)
                {
                    return (await response.Content.ReadAsStringAsync());
                }
            }

            return (null);
        }

        public async Task<string> Delete(string url, string headersJson = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url);
            HttpResponseMessage response = null;

            if (string.IsNullOrEmpty(url) == false)
            {
                SetHeaders(request, headersJson);
                response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode == true)
                {
                    return (await response.Content.ReadAsStringAsync());
                }
            }

            return (null);
        }

        public async Task<string> Delete(string url, Object headersJson = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url);
            HttpResponseMessage response = null;

            if (string.IsNullOrEmpty(url) == false)
            {
                SetHeaders(request, headersJson);
                response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode == true)
                {
                    return (await response.Content.ReadAsStringAsync());
                }
            }

            return (null);
        }

        private void SetHeaders(HttpRequestMessage request, string headersJson)
        {
            Dictionary<string, string> headers = null;
            if (!string.IsNullOrWhiteSpace(headersJson))
            {
                headers = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersJson);

                foreach (KeyValuePair<string, string> header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
        }

        private void SetHeaders(HttpRequestMessage request, Object headersJson)
        {
            Dictionary<string, string> headers = null;

            if (headersJson != null)
            {
                headers = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    JsonConvert.SerializeObject(headersJson)
                );

                foreach (KeyValuePair<string, string> header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
        }
    }
}
