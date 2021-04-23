using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccountInformation.Client.Controllers
{
    public class TransactionController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public TransactionController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult>  Index(string accountId)
        {
            var client = httpClientFactory.CreateClient("APIClient");
            var request = new HttpRequestMessage(HttpMethod.Get, string.Format($"ais/v7/identified2/accounts/{accountId}/transactions?bookingStatus=booked"));

            request.Headers.Add("X-Request-ID", Guid.NewGuid().ToString());
            request.Headers.Add("PSU-IP-Address", "192.168.1.101");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            var transactions = JsonConvert.DeserializeObject<AccountInformation.Client.Models.Transaction>(data);

            ViewData["transactions"] = transactions.BookedWraper.Booked;
            ViewData["accountId"] = accountId;

            return View();
        }

        public async Task<IActionResult> GetTransaction(string accountId, string transactionId) {
            var client = httpClientFactory.CreateClient("APIClient");
            var request = new HttpRequestMessage(HttpMethod.Get, string.Format($"/ais/v7/identified2/accounts/{accountId}/transactions/{transactionId}"));

            request.Headers.Add("X-Request-ID", Guid.NewGuid().ToString());
            request.Headers.Add("PSU-IP-Address", "192.168.1.101");

            var response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) {
                return Content("This transaction's detail is not found, please try later");
            } 

            var data = await response.Content.ReadAsStringAsync();

            return Content(data);
        }
    }
}
