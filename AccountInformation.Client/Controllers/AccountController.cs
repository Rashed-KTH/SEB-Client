using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AccountInformation.Client.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace AccountInformation.Client.Controllers
{
    public class AccountController : Controller
    {
        private const string ACCOUNT_PATH = "ais/v7/identified2/accounts";

        private readonly IHttpClientFactory httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        
        public async Task<IActionResult> Index()
        {
            var client = httpClientFactory.CreateClient("APIClient");
            var request = new HttpRequestMessage(HttpMethod.Get, string.Format(ACCOUNT_PATH));

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            var accountInfo = JsonConvert.DeserializeObject<AccountWrapper>(data);
            ViewData["accounts"] = accountInfo.accounts;

            return View("Account");
        }
    }
}
