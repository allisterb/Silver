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
                    options.AutoResize = true;
                    var layout = new NetworkLayout()
                    {
                        Hierarchical = new NetworkHierarchical()
                        {
                            Enabled = true,
                            LevelSeparation = 300
                        }
                    };
                    var physics = new NetworkPhysics()
                    {
                        HierarchicalRepulsion = new NetworkHierarchicalRepulsion()
                        {
                            NodeDistance = 300
                        }
                    };
                    options.Layout = layout;
                    options.Physics = physics;
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
                    Font = new NetworkFont() { Face = node.Label.FontName },
                    Shape = nodeshape,
                    Size = nodesize,
                });
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
