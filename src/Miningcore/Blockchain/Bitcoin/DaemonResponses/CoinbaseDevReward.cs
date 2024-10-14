using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Miningcore.Blockchain.Bitcoin.DaemonResponses
{
    public class CoinbaseDevReward
    {
        public string ScriptPubkey { get; set; }
<<<<<<< HEAD
        public long   Value        { get; set; }
=======
        public long Value { get; set; }
>>>>>>> 69de0d393ec56f3e0535f3b09f6de93d6299beec
    }

    public class CoinbaseDevRewardTemplateExtra
    {
<<<<<<< HEAD
        public JToken CoinbaseDevReward { get; set; }
    }
}
=======
        [JsonProperty("coinbasedevreward")]
        public JToken CoinbaseDevReward { get; set; }
    }
}
>>>>>>> 69de0d393ec56f3e0535f3b09f6de93d6299beec
