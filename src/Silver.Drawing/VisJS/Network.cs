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

    [JsonProperty("sortMethod", NullValueHandling = NullValueHandling.Ignore)]
    public string? SortMethod { get; set; }

    [JsonProperty("direction", NullValueHandling = NullValueHandling.Ignore)]
    public string? Direction { get; set; }

    [JsonProperty("nodeSpacing", NullValueHandling = NullValueHandling.Ignore)]
    public int? NodeSpacing { get; set; }

    [JsonProperty("levelSeparation", NullValueHandling = NullValueHandling.Ignore)]
    public int? LevelSeparation { get; set; }
}

public class NetworkHierarchicalRepulsion
{
    [JsonProperty("nodeDistance", NullValueHandling = NullValueHandling.Ignore)]
    public int? NodeDistance { get; set; }

    [JsonProperty("avoidOverlap", NullValueHandling = NullValueHandling.Ignore)]
    public double? AvoidOverlap { get; set; }
}

public class NetworkLayout
{
    [JsonProperty("improvedLayout", NullValueHandling = NullValueHandling.Ignore)]
    public bool? ImprovedLayout { get; set; }

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
    public NetworkColor? Color { get; set; }

    [JsonProperty("shape", NullValueHandling = NullValueHandling.Ignore)]
    public string? Shape { get; set; }

    [JsonProperty("font", NullValueHandling = NullValueHandling.Ignore)]
    public NetworkFont? Font { get; set; }
}

public class NetworkOptions
{
    [JsonProperty("autoResize", NullValueHandling = NullValueHandling.Ignore)]
    public bool? AutoResize { get; set; }

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

    [JsonProperty("edges", NullValueHandling = NullValueHandling.Ignore)]
    public NetworkEdgesOptions? Edges { get; set; }

    [JsonProperty("interaction", NullValueHandling = NullValueHandling.Ignore)]
    public NetworkInteraction? Interaction { get; set; }
}

public class NetworkPhysics
{
    [JsonProperty("hierarchicalRepulsion", NullValueHandling = NullValueHandling.Ignore)]
    public NetworkHierarchicalRepulsion? HierarchicalRepulsion { get; set; }
}

public class NetworkSmooth
{
    [JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Enabled { get; set; }

    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public string? Type { get; set; }
}

public class NetworkEdgeArrows
{
    [JsonProperty("to", NullValueHandling = NullValueHandling.Ignore)]
    public NetworkEdgeTo? To { get; set; }
}

public class NetworkEdgesOptions
{
    [JsonProperty("arrows", NullValueHandling = NullValueHandling.Ignore)]
    public NetworkEdgeArrows? Arrows { get; set; }

    [JsonProperty("smooth", NullValueHandling = NullValueHandling.Ignore)]
    public NetworkSmooth? Smooth { get; set; }
}


public class NetworkEdgeTo
{
    [JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Enabled { get; set; }
}


public class NetworkInteraction
{
    [JsonProperty("navigationButtons", NullValueHandling = NullValueHandling.Ignore)]
    public bool? NavigationButtons { get; set; }

    [JsonProperty("keyboard", NullValueHandling = NullValueHandling.Ignore)]
    public NetworkInteractionKeyboard? Keyboard { get; set; }
}

public class NetworkInteractionKeyboard
{
    [JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Enabled { get; set; }

    [JsonProperty("speed", NullValueHandling = NullValueHandling.Ignore)]
    public NetworkInteractionKeyboardSpeed? Speed { get; set; }

    [JsonProperty("bindToWindow", NullValueHandling = NullValueHandling.Ignore)]
    public bool? BindToWindow { get; set; }
}


public class NetworkInteractionKeyboardSpeed
{
    [JsonProperty("x")]
    public int X { get; set; }

    [JsonProperty("y")]
    public int Y { get; set; }

    [JsonProperty("zoom")]
    public double Zoom { get; set; }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class NetworkColorHighlight
{
    [JsonProperty("border", NullValueHandling = NullValueHandling.Ignore)]
    public string? Border { get; set; }

    [JsonProperty("background", NullValueHandling = NullValueHandling.Ignore)]
    public string? Background { get; set; }
}

public class NetworkColorHover
{
    [JsonProperty("border", NullValueHandling = NullValueHandling.Ignore)]
    public string? Border { get; set; }

    [JsonProperty("background", NullValueHandling = NullValueHandling.Ignore)]
    public string? Background { get; set; }
}

public class NetworkColor
{
    [JsonProperty("border", NullValueHandling = NullValueHandling.Ignore)]
    public string? Border { get; set; }

    [JsonProperty("background", NullValueHandling = NullValueHandling.Ignore)]
    public string? Background { get; set; }

    [JsonProperty("highlight", NullValueHandling = NullValueHandling.Ignore)]
    public NetworkColorHighlight? Highlight { get; set; }

    [JsonProperty("hover", NullValueHandling = NullValueHandling.Ignore)]
    public NetworkColorHover? Hover { get; set; }
}


/*
 * // these are all options in full.
var options = {
  nodes:{
    borderWidth: 1,
    borderWidthSelected: 2,
    brokenImage:undefined,
    chosen: true,
    color: {
      border: '#2B7CE9',
      background: '#97C2FC',
      highlight: {
        border: '#2B7CE9',
        background: '#D2E5FF'
      },
      hover: {
        border: '#2B7CE9',
        background: '#D2E5FF'
      }
    },
    opacity: 1,
    fixed: {
      x:false,
      y:false
    },
    font: {
      color: '#343434',
      size: 14, // px
      face: 'arial',
      background: 'none',
      strokeWidth: 0, // px
      strokeColor: '#ffffff',
      align: 'center',
      multi: false,
      vadjust: 0,
      bold: {
        color: '#343434',
        size: 14, // px
        face: 'arial',
        vadjust: 0,
        mod: 'bold'
      },
      ital: {
        color: '#343434',
        size: 14, // px
        face: 'arial',
        vadjust: 0,
        mod: 'italic',
      },
      boldital: {
        color: '#343434',
        size: 14, // px
        face: 'arial',
        vadjust: 0,
        mod: 'bold italic'
      },
      mono: {
        color: '#343434',
        size: 15, // px
        face: 'courier new',
        vadjust: 2,
        mod: ''
      }
    },
    group: undefined,
    heightConstraint: false,
    hidden: false,
    icon: {
      face: 'FontAwesome',
      code: undefined,
      weight: undefined,
      size: 50,  //50,
      color:'#2B7CE9'
    },
    image: undefined,
    imagePadding: {
      left: 0,
      top: 0,
      bottom: 0,
      right: 0
    },
    label: undefined,
    labelHighlightBold: true,
    level: undefined,
    mass: 1,
    physics: true,
    scaling: {
      min: 10,
      max: 30,
      label: {
        enabled: false,
        min: 14,
        max: 30,
        maxVisible: 30,
        drawThreshold: 5
      },
      customScalingFunction: function (min,max,total,value) {
        if (max === min) {
          return 0.5;
        }
        else {
          let scale = 1 / (max - min);
          return Math.max(0,(value - min)*scale);
        }
      }
    },
    shadow:{
      enabled: false,
      color: 'rgba(0,0,0,0.5)',
      size:10,
      x:5,
      y:5
    },
    shape: 'ellipse',
    shapeProperties: {
      borderDashes: false, // only for borders
      borderRadius: 6,     // only for box shape
      interpolation: false,  // only for image and circularImage shapes
      useImageSize: false,  // only for image and circularImage shapes
      useBorderWithImage: false,  // only for image shape
      coordinateOrigin: 'center'  // only for image and circularImage shapes
    }
    size: 25,
    title: undefined,
    value: undefined,
    widthConstraint: false,
    x: undefined,
    y: undefined
  }
}
*/
