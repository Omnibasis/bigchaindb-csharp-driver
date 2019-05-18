using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Omnibasis.BigchainCSharp_Test
{
    [Serializable]
    public class TestAsset
    {
        [JsonProperty]
        public string msg { get; set; }
        [JsonProperty]
        public string city { get; set; }
        [JsonProperty]
        public string temperature { get; set; }
        [JsonProperty]
        public DateTime datetime { get; set; }
    }
}
