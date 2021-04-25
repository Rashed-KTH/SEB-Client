using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AccountInformation.Client.Common
{
    public class TokenManager
    {
        private readonly IHttpClientFactory httpClientFactory;

        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly AppConfig appConfig;

        public TokenManager(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IOptions<AppConfig> appConfig)
        {
            this.httpClientFactory = httpClientFactory;
            this.httpContextAccessor = httpContextAccessor;
            this.appConfig = appConfig.Value;
            
        }

        /// <summary>
        /// Get token information using "Authorization code"
        /// </summary>
        /// <returns>
        /// A dictionay containing access_token, refresh_token, expires_in as key
        /// </returns>
        public async Task<Dictionary<string, string>> GetTokenInfoAsync(string code)
        {
            var formData = GenerateFormData(AppConstant.FLOW);
            formData.Add(new KeyValuePair<string, string>(AppConstant.CODE, code));
            formData.Add(new KeyValuePair<string, string>(AppConstant.REDIRECT_URI, appConfig.RedirectUri));
            httpContextAccessor.HttpContext.Request.Headers.Clear();
            var tokenInfo = await getTokenPayloadAsync(formData);

            return (tokenInfo);
        }

        /// <summary>
        /// Get token information using "refresh_token"
        /// </summary>
        /// <returns>
        /// A dictionay containing access_token, refresh_token, expires_in as key
        /// </returns>
        public async Task<Dictionary<string, string>> GetRefreshTokenInfoAsync(string refresh_token)
        {
            var formData = GenerateFormData(AppConstant.REFRESH_TOKEN);
            formData.Add(new KeyValuePair<string, string>(AppConstant.REFRESH_TOKEN, refresh_token));
            var tokenInfo = await getTokenPayloadAsync(formData);

            return tokenInfo;
        }

        private async Task<Dictionary<string, string>> getTokenPayloadAsync(List<KeyValuePair<string, string>> formData)
        {
            var client = httpClientFactory.CreateClient("APIClient");
            var request = new HttpRequestMessage(HttpMethod.Post, string.Format("/mga/sps/oauth/oauth20/token"));

            request.Content = new FormUrlEncodedContent(formData);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            if (data == null) return null;

            Dictionary<string, string> tokenInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

            return tokenInfo;
        }

        private List<KeyValuePair<string, string>> GenerateFormData(string grantType)
        {
            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>(AppConstant.CLIENT_SECRET, appConfig.ClientSecret));
            formData.Add(new KeyValuePair<string, string>(AppConstant.CLIENT_ID, appConfig.ClientId));
            formData.Add(new KeyValuePair<string, string>(AppConstant.GRANT_TYPE, grantType));
            return formData;
        }
    }
}
