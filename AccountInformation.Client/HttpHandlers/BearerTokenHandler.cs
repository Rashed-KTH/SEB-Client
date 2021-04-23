using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AccountInformation.Client.HttpHandlers
{
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BearerTokenHandler(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            this._httpClientFactory = httpClientFactory;
            this._httpContextAccessor = httpContextAccessor;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await GetAccessTokenAsync();

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var accessToken = await _httpContextAccessor
                       .HttpContext.GetTokenAsync("access_token");

            if (accessToken == null) {
                var authClient = _httpClientFactory.CreateClient("APIClient");
                var response = authClient.GetAsync("https://api-sandbox.sebgroup.com/mga/sps/oauth/oauth20/authorize?" +
                    "scope=psd2_accounts&scope=psd2_payments&client_id=TsZlVNuzDpmch7m0nQGB" +
                    "&redirect_uri=https://localhost:5002&response_type=code");

                return response.ToString();
            }
            return null;
        }
    }
}
