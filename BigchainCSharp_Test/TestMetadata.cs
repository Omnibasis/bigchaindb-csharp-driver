using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Omnibasis.BigchainCSharp_Test
{
    [Serializable]
    public class TestMetadata
    {
        [JsonProperty]
        public string msg { get; set; }
    }
}
