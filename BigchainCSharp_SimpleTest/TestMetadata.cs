using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

[Serializable]
public class TestMetadata
{
    [JsonProperty]
    public string msg { get; set; }
}
