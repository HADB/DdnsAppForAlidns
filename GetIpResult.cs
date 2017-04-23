using Newtonsoft.Json;

namespace DdnsAppForAlidns
{
    public class GetIpResult
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }
    }
}
