using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Msagl.Drawing;

namespace Silver.Drawing.VisJS
{
    public class VisJS
    {
        public static Network Draw(Graph graph, string width="100%", string height="600px")
        {
            Network network = new Network();
            var options = new NetworkOptions();
            options.Width = width;
            options.Height = height;
            List<NetworkNode> nodes = new List<NetworkNode>();
            List<NetworkEdge> edges = new List<NetworkEdge>();
            string shape = "circle";
            
            switch (graph.Kind)
            {
                case "cfg":
                    shape = "box";
                    break;
            }
            
            foreach(var node in graph.Nodes)
            {
                nodes.Add(new NetworkNode() 
                { 
                    Id = node.Id, 
                    Label = node.LabelText, 
                    Font = new NetworkFont() { Face = node.Label.FontName },
                    Shape = shape,
                    
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
