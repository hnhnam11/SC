using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ScamCheck
{
    public sealed class ApiBridge
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _analyzeEndpoint;

        public ApiBridge(string baseUrl, HttpClient httpClient = null)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("Base URL is required.", nameof(baseUrl));
            }

            _httpClient = httpClient ?? new HttpClient();
            _analyzeEndpoint = new Uri(new Uri(baseUrl.TrimEnd('/') + "/"), "analyze");
        }

        public async Task<string> AnalyzeAsync(string message, CancellationToken cancellationToken = default)
        {
            using (var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["message"] = message ?? string.Empty
            }))
            using (var response = await _httpClient.PostAsync(_analyzeEndpoint, content, cancellationToken).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }
    }
}
