using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VDS.RDF;
using VDS.RDF.Parsing;
using Newtonsoft.Json;
using DataQualitySemWebLib;

// Current tasks: remove: multiple ways of loading URI's
// No clear way to load graphs?? 
// Need to be able to load any URI.  IE, non-dbpedia
// Easier transition between auto-add and pick to add
// Find data quality problems
        // Lots to do here
// Visualize the "values"
// Intial graph finds lots of things that don't belong. Is there some way to eliminate those?
// Right now we have 1 class, Node that rules them all.  Could use something like an adapter pattern so that
// there is one class that is exposed and another class that facilitates the information from the rdf graphs

namespace Visualize.Controllers
{

    public class VisualizeController : Controller
    {
        [HttpPost]
        public JsonResult GetNext(List<Node> visualization, string Graphs )// List<Node> visualization)  // Dictionary<string, List<FlatPropertyValueEntries>>
        {
            //var nodes = (List<Node>)Newtonsoft.Json.JsonConvert.DeserializeObject(visualization);

            // NodeContainer vis = GetNodeContainer(NodeHelper.GetNodeIncrementer() + visualization.Count );
            var nc = new NodeContainer();
            nc.json = visualization;
            //CreateRandomNode()

            // AppendToRandomNode(nc);

            return Json(nc.json);
            
        }

        [HttpGet]
        public JsonResult Initialize(string InitialURI)
        {
            var nc = new NodeContainer();
            nc = CreateInitialNode(InitialURI);
            return Json(nc, JsonRequestBehavior.AllowGet); //, Newtonsoft.Json.Formatting.Indented);
        }

        private NodeContainer CreateInitialNode(string InitialURI)
        {
            var nc = new NodeContainer();
            NodeBinder n = FindData(InitialURI);
            n.Bind(nc);
            var topNode = nc.json[0];

            // ok, we have our intial node all set up
            // now we need to append some more nodes

            // create a random node from the entry list

            int numIterations = 10;
            for (var iterCount = 0; iterCount < numIterations; iterCount++)
            {
                // let's do this a couple more times
                var x = CreateRandomNode(nc.Graphs[InitialURI]);

                if (x != null)
                {
                    // but first, let's ensure uniqueness.  this is bad practice.  the add on the list should do this.
                    if (EnsureUniqueness(topNode, x))
                    {
                        nc.json.Add(x);
                        // append to the adjacency list for the top level node
                        topNode.adjacencies.Add(x.id);
                    }
                }
            }

            return nc;
        }
        private bool EnsureUniqueness(Node existingNode, Node newNode)
        {
            if (newNode == null)
                return false; // definitely not unique
            foreach(var el in existingNode.adjacencies)
            {
                if (el == newNode.id)
                    return false;
            }
            return true;
        }

        

        private Node CreateRandomNode(List<FlatPropertyValueEntries> entries)
        {
            var node = new Node();
            // random number between 0 and entry list count - 1
            var random = new Random();
            var nodeNumber = random.Next( 0, entries.Count-1);

            // todo: need to analyze this propertyname for violations
            string propURI = entries[nodeNumber].PropertyName;

            // now get the value of the label for this property
            // first we need to load the URI for the property entries
            // make a graph
            var g = new Graph();

            var uriVal = new URIValidator();
            var URIValResult = uriVal.Validate(propURI);
            string NodeName = ""; 
            if (URIValResult.Violation == SemWebDataQualityViolations.None)
            {
                propURI = URIValResult.ValidatedSearch;

                // load the graph
                // UriLoader.Load(g, new Uri(propURI), new RdfXmlParser()); 
                UriLoader.Load(g, new Uri(propURI));

                // need to figure out what went wrong...
                if (g.IsEmpty)
                    return null;

                // we could create a constructor overload to combine these 3 steps...
                var gt = new GraphTranslator(g);
                var propEntries = gt.GetTranslation();

                var propExt = new PropertyNameExtractor(propEntries);

                var propRes = propExt.FindBasicLabelURI();
                if (propRes.Violation != SemWebDataQualityViolations.None)
                {
                    SemWebDataQualityViolationProcessor.Log(node.data, propRes.Violation);
                }
                string labelURI = propRes.Result;

                var valExt = new PropertyValueExtractor(propEntries);
                var valueRes = valExt.GetResults(labelURI);
                if (valueRes.Violation != SemWebDataQualityViolations.None)
                {
                    SemWebDataQualityViolationProcessor.Log(node.data, valueRes.Violation);
                }
                NodeName = valueRes.Results.Take(1).FirstOrDefault();

            }
            else
            {
                if( URIValResult.Violation == SemWebDataQualityViolations.UnableToDereferenceURI )
                {
                    SemWebDataSuggestionGenerator swdsg = new SemWebDataSuggestionGenerator(propURI);
                    if (swdsg.Result.Value.Length > 0)
                    {
                        SemWebDataQualityViolationProcessor.Log(node.data, SemWebDataQualityViolations.UnableToDereferenceURIFoundSearchedAlternative);
                        propURI = swdsg.Result.Source;
                        NodeName = swdsg.Result.Value;
                    }
                    else
                    {
                        SemWebDataQualityViolationProcessor.Log(node.data, URIValResult.Violation);
                    }
                        
                }
                
            }

            node.name = NodeName;
            node.data.URI = propURI;
            
            foreach(var el in entries[nodeNumber].Values)
            {
                // todo: this might be a problem... we're not telling anything if it is URINode or not
                // also need to analyze the values as they are retrieved.
                // also need to put any violations on the node itself so that it can be processed
                
                node.data.text.Add(el.Value);
            }

            if(DataQualityHelper.ContainsDuplicateResults(node.data.text))
            {
                SemWebDataQualityViolationProcessor.Log(node.data, SemWebDataQualityViolations.MultipleLabelValuesFound);
            }
            
            return node;
        }

        private NodeBinder FindData(string URI)
        { 
            // create a new node
            var NewNode = new Node();

            // create a URI Validator
            var urival = new URIValidator();
            string validURI = "";
            
            // validate the URI
            var URIValResult = urival.Validate(URI);
            if (URIValResult.Violation == SemWebDataQualityViolations.None)
                validURI = URIValResult.ValidatedSearch;
            else
            {
                SemWebDataQualityViolationProcessor.Log(NewNode.data, URIValResult.Violation);
            }
            
            // make a graph
            var g = new Graph();

            // load the graph
            UriLoader.Load(g, new Uri(validURI));

            // create a translator for the graph
            var gt = new GraphTranslator(g);

            // basically, flatten the graph and create properties and values for those properties
            // the properties are URI's.  The values can be literals or URIs
            var lfpve = gt.GetTranslation();
            
            // create an extractor to be able to retrieve property names from the flattened graph
            var pne = new PropertyNameExtractor(lfpve);

            // find the label value.  This is a pretty specific function, but probably commonly used
            var labelRes = pne.FindBasicLabelURI();
            if(labelRes.Violation != SemWebDataQualityViolations.None)
            {
                SemWebDataQualityViolationProcessor.Log(NewNode.data, labelRes.Violation);
            }
            string labelURI = labelRes.Result;
            // create a value extractor to get a specific value for one of the properties in the flattened graph
            var valueExt = new PropertyValueExtractor(lfpve);

            // get the values for a URI.  We are getting the values for one of the properties in the 
            // flattened graph.
            var valueResult = valueExt.GetResults(labelURI);

            string strLabel;

            // we have a violoation.  Specifically, there are duplicate values found.  Use the 
            // most frequently recurring value.
            if (valueResult.Violation != SemWebDataQualityViolations.None)
            {
                SemWebDataQualityViolationProcessor.Log(NewNode.data, valueResult.Violation);
                strLabel = DataQualityHelper.GetMostFrequentResult(valueResult.Results);
            }
            else
                strLabel = valueResult.Results[0];

            // all that work just to get a label for the passed in URI.
            // please note that this will only work for the top level menu.
            // for other nodes, a different procedure will be needed, most likely
            NewNode.name = strLabel;

            NewNode.data.URI = URI;
            NewNode.data.text.Add(NewNode.name);
            var nb = new NodeBinder();
            nb.node = NewNode;
            nb.entriesList = lfpve;
            nb.URI = NewNode.data.URI; // this seems redundant to above

            return nb;
        }

        //
        // GET: /Visualize/
        // Initial view of the screen
        public ViewResult Index()
        {
            return View();
        }
	}
}