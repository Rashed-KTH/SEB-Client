using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AccountInformation.Client.Models
{
    public class Transaction
    {
        [JsonProperty("transactions")]
        public BookedWrapper BookedWraper { get; set; }
    }

    public class BookedWrapper
    {
        [JsonProperty("booked")]
        public IList<Booked> Booked { get; set; }
    }

    public class Booked {
        public string TransactionId { get; set; }
    }
}
