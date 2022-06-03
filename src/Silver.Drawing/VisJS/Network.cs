namespace Silver.Drawing.VisJS;

using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

public class Network
{
    [JsonProperty("options")]
    public NetworkOptions? Options { get; set; } = new NetworkOptions() { Width = "100%", Height = "600px"};

    [JsonProperty("nodes")]
    public List<NetworkNode>? Nodes { get; set; }

    [JsonProperty("edges")]
    public List<NetworkEdge>? Edges { get; set; }

    [JsonIgnore]
    public string Width => Options?.Width ?? "100%";

    [JsonIgnore]
    public string Height => Options?.Height ?? "600px";

    public static Network Load(string data) => JsonConvert.DeserializeObject<Network>(data)!;

    public static Network LoadFrom(string f) => JsonConvert.DeserializeObject<Network>(File.ReadAllText(f))!;
}

public class NetworkEdge
{
    [JsonProperty("from", NullValueHandling = NullValueHandling.Ignore)]
    public string? From { get; set; }

    [JsonProperty("to", NullValueHandling = NullValueHandling.Ignore)]
    public string? To { get; set; }

    [JsonProperty("arrows", NullValueHandling = NullValueHandling.Ignore)]
    public string? Arrows { get; set; }

    [JsonProperty("physics", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Physics { get; set; }

    [JsonProperty("smooth", NullValueHandling = NullValueHandling.Ignore)]
    public NetworkSmooth? Smooth { get; set; }
}

public class NetworkFont
{
    [JsonProperty("face", NullValueHandling = NullValueHandling.Ignore)]
    public string? Face { get; set; }

    [JsonProperty("align", NullValueHandling = NullValueHandling.Ignore)]
    public string? Align { get; set; }
}

public class NetworkHierarchical
{
    [JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Enabled { get; set; }

    [JsonProperty("levelSeparation", NullValueHandling = NullValueHandling.Ignore)]
    public int? LevelSeparation { get; set; }
}

public class NetworkHierarchicalRepulsion
{
    [JsonProperty("nodeDistance", NullValueHandling = NullValueHandling.Ignore)]
    public int? NodeDistance { get; set; }
}

public class NetworkLayout
{
    [JsonProperty("hierarchical", NullValueHandling = NullValueHandling.Ignore)]
    public NetworkHierarchical? Hierarchical { get; set; }
}

public class NetworkNode
{
    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public string? Id { get; set; }

    [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
    public int? Size { get; set; }

    [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
    public string? Label { get; set; }

    [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
    public string? Color { get; set; }

    [JsonProperty("shape", NullValueHandling = NullValueHandling.Ignore)]
    public string? Shape { get; set; }

    [JsonProperty("font", NullValueHandling = NullValueHandling.Ignore)]
    public NetworkFont? Font { get; set; }
}

public class NetworkOptions
{
    [JsonProperty("manipulation", NullValueHandling = NullValueHandling.Ignore)]
    public bool Manipulation { get; set; }

    [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
    public string? Width { get; set; }

    [JsonProperty("height", NullValueHandling = NullValueHandling.Ignore)]
    public string? Height { get; set; }

    [JsonProperty("layout", NullValueHandling = NullValueHandling.Ignore)]
    public NetworkLayout? Layout { get; set; }

    [JsonProperty("physics", NullValueHandling = NullValueHandling.Ignore)]
    public NetworkPhysics? Physics { get; set; }
}

public class NetworkPhysics
{
    [JsonProperty("hierarchicalRepulsion", NullValueHandling = NullValueHandling.Ignore)]
    public NetworkHierarchicalRepulsion? HierarchicalRepulsion { get; set; }
}

public class NetworkSmooth
{
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public string? Type { get; set; }
}