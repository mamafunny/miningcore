using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Miningcore.Blockchain.Bitcoin.DaemonResponses
{
    public class Community
    {
        public string Payee { get; set; }
        public string Script { get; set; }
        public long Amount { get; set; }
    }

    public class CommunityBlockTemplateExtra
    {
        public JToken Community { get; set; }

        [JsonProperty("community_payments_started")]
        public bool CommunityPaymentsStarted { get; set; }

	[JsonExtensionData]
	public IDictionary<string, object> Extra { get; set; }
    }
}
