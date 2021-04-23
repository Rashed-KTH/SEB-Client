using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AccountInformation.Client.Models
{
    public class Account
    {
        public string ResourceId { get; set; }
        public string OwnerName { get; set; }
    }

    public class AccountWrapper
    {
        [JsonProperty("accounts")]
        public IList<Account> accounts { get; set; }
    }
}
