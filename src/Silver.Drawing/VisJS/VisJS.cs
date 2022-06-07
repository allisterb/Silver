using System;
using Microsoft.Msagl.Drawing;

namespace Silver.Drawing.VisJS
{
    public class VisJS
    {
        public static Network Draw(Graph graph, string width="100%", string height="600px")
        {
            var network = new Network();
            var options = new NetworkOptions();
            options.Width = width;
            options.Height = height;
            string? nodeshape = null;
            int? nodesize = null;
            
            switch (graph.Kind)
            {
                case "cfg":
                case "cg":
                    options.AutoResize = false;
                    var layout = new NetworkLayout()
                    {
                        ImprovedLayout = true,
                        Hierarchical = new NetworkHierarchical()
                        {
                            Enabled = true,
                            SortMethod = "directed",
                            Direction = "UD",
                            NodeSpacing = 300,
                            LevelSeparation = 300,
                        }
                    };
                    var physics = new NetworkPhysics()
                    {
                        HierarchicalRepulsion = new NetworkHierarchicalRepulsion()
                        {
                            AvoidOverlap = 1.0,
                            NodeDistance = 300
                        }
                    };
                    var edgesOptions = new NetworkEdgesOptions()
                    {
                        Arrows = new NetworkEdgeArrows() { To = new NetworkEdgeTo() { Enabled = true } },
                        Smooth = new NetworkSmooth() { Enabled = true }
                    };
                    var interaction = new NetworkInteraction()
                    {
                        NavigationButtons = true
                    };
                    options.Layout = layout;
                    options.Physics = physics;
                    options.Edges = edgesOptions;
                    options.Interaction = interaction;
                    nodeshape = "box";
                    nodesize = 150;
                    break;
            }

            List<NetworkNode> nodes = new List<NetworkNode>();
            List<NetworkEdge> edges = new List<NetworkEdge>();
            foreach (var node in graph.Nodes)
            {
                nodes.Add(new NetworkNode()
                {
                    Id = node.Id,
                    Label = node.LabelText,
                    Font = new NetworkFont() { Face = "monospace", Align = "left" },
                    Shape = nodeshape,
                    Size = nodesize,
                    Color = new NetworkColor()
                    {
                        Background = ( node.Attr.FillColor == Color.Transparent || node.Attr.FillColor == Color.Black) ? null : node.Attr.FillColor.ToString().Trim('"')
                    }
                }); ;
            }
            
            foreach(var edge in graph.Edges)
            {
                edges.Add(new NetworkEdge()
                {
                    From = edge.Source,
                    To = edge.Target,
                });
            }

            network.Nodes = nodes;
            network.Edges = edges;
            network.Options = options;
            return network;
        }
    }
}
