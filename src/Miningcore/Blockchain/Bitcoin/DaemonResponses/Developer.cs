using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Miningcore.Blockchain.Bitcoin.DaemonResponses
{
    public class Developer
    {
        public string Payee { get; set; }
        public string Script { get; set; }
        public long Amount { get; set; }
    }

    public class DeveloperBlockTemplateExtra
    {
        public JToken Developer{ get; set; }

        [JsonProperty("developer_payments_started")]
        public bool DeveloperPaymentsStarted { get; set; }

	[JsonExtensionData]
	public IDictionary<string, object> Extra { get; set; }
    }
}
