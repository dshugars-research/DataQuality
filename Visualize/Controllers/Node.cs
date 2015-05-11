using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataQualitySemWebLib;

namespace Visualize.Controllers
{
    // this class does not seem necessary
    public class NodeContainer
    {

        public List<Node> json { get; set; }
        public NodeContainer()
        {
            json = new List<Node>();
            Graphs = new Dictionary<string, List<FlatPropertyValueEntries>>();
        }
        public Dictionary<string, List<FlatPropertyValueEntries>> Graphs { get; set; }

    }

    // class to bind entries to the nodecontainer. also binds node to nodecontainer
    public class NodeBinder
    {
        public string URI;
        public Node node;
        public List<FlatPropertyValueEntries> entriesList;
        public void Bind(NodeContainer nc)
        {
            if (entriesList.Count == 0)
                throw new Exception("Entries count is 0.");
            if (URI.Length == 0)
                throw new Exception("URI is not set.");
            if (node == null)
                throw new Exception("Node is null.");
            
            nc.Graphs.Add(URI, entriesList);

            nc.json.Add(node);
        }
    }
    public class Node 
    {
        public string id
        {
            get;
            set;
        }
        public string name { get; set; }

        public data data { get; set; }
        
        public List<string> adjacencies { get; set; }
        public Node()
        {
            data = new data();
            adjacencies = new List<string>();
            id = Guid.NewGuid().ToString();
            // shouldn't this do this: id = new Guid();

        }
        // public NodeContainer adjacencies = new NodeContainer();
    }
    public class data : IViolationMessageStore
    {
        // public bool overridable = true;
        // public string canvas_visibility = "hidden";
        public int height { get; set; }
        public int width { get; set; }
        public string type { get; set; }
        public string color { get; set; }
        public int dim { get; set; }
        public string URI { get; set; }
        public bool clicked { get; set; }

        // a list of values for the node.
        public List<string> text { get; set; }
        public List<string> Messages { get; set; }
        public void AddViolationMessage(string msg)
        {
            Messages.Add(msg);
        }
        public data()
        {
            height = 50;
            width = 90;
            type = "circle";
            color = "#ccb";
            dim = 10;
            URI = "";
            clicked = false;
            text = new List<string>();
            Messages = new List<string>();
        }
        // public CanvasStyles CanvasStyles = new CanvasStyles();
    }
    // is this class necessary?
    public class CanvasStyles
    {
        public string visibility = "hidden";
    }
    public static class NodeHelper
    {
        public static int GetNodeIncrementer()
        {
            return 3;
        }
    }

    public class AdjacentNodes
    {
        public List<string> adjacencies = new List<string>();
        public void Add(string val)
        {

            adjacencies.Add(val);
        }

    }
    
}