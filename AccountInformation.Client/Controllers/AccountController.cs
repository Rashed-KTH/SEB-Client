using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AccountInformation.Client.Models;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json;
using AccountInformation.Client.Common;
using Microsoft.Extensions.Options;

namespace AccountInformation.Client.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly AppConfig appConfig;

        public AccountController(IHttpClientFactory httpClientFactory, IOptions<AppConfig> appConfig)
        {
            this.httpClientFactory = httpClientFactory;
            this.appConfig = appConfig.Value;
        }

        public async Task<IActionResult> Index(string code)
        {
            if (code == null)
            {
                var url = string.Format(@"https://api-sandbox.sebgroup.com/mga/sps/oauth/oauth20/authorize?scope=psd2_accounts&scope=psd2_payments&client_id=CoL0wYTLDj9zDPfEC50O&redirect_uri={0}&response_type=code",
                    HttpUtility.UrlEncode(@"https://localhost:5001"));

                var ps = new ProcessStartInfo(url)
                {
                    UseShellExecute = true
                };
                Process.Start(ps);
            } else if(code != null){
                var accessToken = await GetAccessTokenAsync(code);

                var client = httpClientFactory.CreateClient("APIClient");
                var request = new HttpRequestMessage(HttpMethod.Get, string.Format("ais/v7/identified2/accounts"));

                request.Headers.Add("X-Request-ID", Guid.NewGuid().ToString());
                request.Headers.Add("PSU-IP-Address", "192.168.1.101");
                request.Headers.Add("Authorization", $"Bearer {accessToken}");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();
                var accountInfo = JsonConvert.DeserializeObject<AccountWrapper>(data);
                ViewData["accounts"] = accountInfo.accounts;
            }
            return View();
        }

        private async Task<string> GetAccessTokenAsync(string code)
        {
            var client = httpClientFactory.CreateClient("APIClient");
            var request = new HttpRequestMessage(HttpMethod.Post, string.Format("/mga/sps/oauth/oauth20/token"));

            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("client_secret", appConfig.ClientSecret));
            formData.Add(new KeyValuePair<string, string>("client_id", appConfig.ClientId));
            formData.Add(new KeyValuePair<string, string>("code", code));
            formData.Add(new KeyValuePair<string, string>("redirect_uri", appConfig.RedirectUri));
            formData.Add(new KeyValuePair<string, string>("grant_type", appConfig.GrantType));

            request.Content = new FormUrlEncodedContent(formData);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            if (data == null)
                return null;

            Dictionary<string, string> tokenInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var accessToken = tokenInfo["access_token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                Response.Cookies.Append("access_token", accessToken);
            }
            return (accessToken);
        }
    }
}
