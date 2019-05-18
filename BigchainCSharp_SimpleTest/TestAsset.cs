using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

[Serializable]
public class TestAsset
{
    [JsonProperty]
    public string msg { get; set; }
    [JsonProperty]
    public string city { get; set; }
    [JsonProperty]
    public int temperature { get; set; }
    [JsonProperty]
    public DateTime datetime { get; set; }
}
