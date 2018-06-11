using Newtonsoft.Json;
using System;

namespace drop_token.Models
{
    public class GetMoveResponse
    {
        public string type { get; set; }
        public string player { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? column { get; set; }
    }
}