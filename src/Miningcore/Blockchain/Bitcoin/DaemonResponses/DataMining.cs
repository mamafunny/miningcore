using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Miningcore.Blockchain.Bitcoin.DaemonResponses
{
    public class DataMining
    {
        public string Payee { get; set; }
        public string Script { get; set; }
        public long Amount { get; set; }
    }

    public class DataMiningBlockTemplateExtra
    {
        public JToken DataMining { get; set; }

        [JsonProperty("datamining_payments_started")]
        public bool DataMiningPaymentsStarted { get; set; }

	[JsonExtensionData]
	public IDictionary<string, object> Extra { get; set; }
    }
}
