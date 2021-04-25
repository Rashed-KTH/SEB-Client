using System.Net.Http;
using System.Threading.Tasks;
using AccountInformation.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccountInformation.Client.Controllers
{
    public class TransactionController : Controller
    {
        private const string TRANSACTION_PATH = @"ais/v7/identified2/accounts/{0}/transactions?bookingStatus=booked";
        private const string TRANSACTION_DETAIL_PATH = @"/ais/v7/identified2/accounts/{0}/transactions/{1}";

        private readonly IHttpClientFactory httpClientFactory;

        public TransactionController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult>  Index(string accountId)
        {
            var client = httpClientFactory.CreateClient("APIClient");
            var request = new HttpRequestMessage(HttpMethod.Get, string.Format(TRANSACTION_PATH, accountId));

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            if (data == null) return Content("Empty payload returned");
            var transactions = JsonConvert.DeserializeObject<Transaction>(data);

            ViewData["transactions"] = transactions.BookedWraper.Booked;
            ViewData["accountId"] = accountId;

            return View();
        }

        public async Task<IActionResult> GetTransactionDetail(string accountId, string transactionId) {
            var client = httpClientFactory.CreateClient("APIClient");
            var request = new HttpRequestMessage(HttpMethod.Get, string.Format(TRANSACTION_DETAIL_PATH, accountId, transactionId));

            var response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) {
                return Content("Transaction's detail is not found, please try later");
            }
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();

            return Content(data);
        }
    }
}
