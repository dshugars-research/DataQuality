using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace DataQualitySemWebLib
{
    public class FlatPropertyValue
    {
        public bool IsURIRef { get; set; }

        public string Value { get; set; }
        public FlatPropertyValue()
        {
            IsURIRef = false;
            Value = "";
        }
    }
    public class FlatPropertyValueEntries
    {
        public string PropertyName { get; set; }
        public List<FlatPropertyValue> Values { get; set; }
        public FlatPropertyValueEntries()
        {
            Values = new List<FlatPropertyValue>();
        }
    }
    public class GraphTranslator
    {
        private Graph _g;
        public GraphTranslator(Graph g)
        {
            _g = g;
        }
        public List<FlatPropertyValueEntries> GetTranslation()
        {
            var lfpvg = new List<FlatPropertyValueEntries>();
            foreach (var el in _g.Triples)
            {
                // predicate is the property. These are going to be unresolved URI's
                // object is the values or potential values; objects NodeType is literal or URI Node
                //if (el.Predicate.ToString().Contains("label"))
                //    Console.WriteLine("here");
                if (lfpvg.FindAll(x => x.PropertyName.Contains(el.Predicate.ToString())).Count == 0)
                {
                    var fpvg = new FlatPropertyValueEntries();
                    fpvg.PropertyName = el.Predicate.ToString();
                    if( ValidateProptertyName(fpvg.PropertyName) == true)
                    {
                        var fpv = new FlatPropertyValue();
                        if (el.Object.NodeType == NodeType.Literal)
                        {
                            fpv.IsURIRef = false;
                            fpv.Value = el.Object.ToString();
                            fpvg.Values.Add(fpv);
                            lfpvg.Add(fpvg);
                        }
                        else if(el.Object.NodeType == NodeType.Uri)
                        {
                            fpv.IsURIRef = true;
                            fpv.Value = el.Object.ToString();
                            fpvg.Values.Add(fpv);
                            lfpvg.Add(fpvg);
                        }
                    }
                }
                else
                {
                    var fpv = new FlatPropertyValue();
                    if (el.Object.NodeType == NodeType.Literal)
                    {
                        fpv.IsURIRef = false;
                        fpv.Value = el.Object.ToString();
                        lfpvg.Find(x => x.PropertyName.Contains(el.Predicate.ToString())).Values.Add(fpv);
                    }
                    else if (el.Object.NodeType == NodeType.Uri)
                    {
                        fpv.IsURIRef = true;
                        fpv.Value = el.Object.ToString();
                        lfpvg.Find(x => x.PropertyName.Contains(el.Predicate.ToString())).Values.Add(fpv);
                    }
                }
            }
            return lfpvg;
        }
        private bool ValidateProptertyName( string propertyName )
        {
            // maybe turn this into a database lookup someday?
            var InvalidPropertyNames = new List<string>();
            InvalidPropertyNames.Add("http://www.w3.org/1999/xhtml/vocab#alternate");
            InvalidPropertyNames.Add("http://www.w3.org/1999/xhtml/vocab#stylesheet");
            InvalidPropertyNames.Add("http://xmlns.com/foaf/0.1/primarytopic");
            if (InvalidPropertyNames.FindAll(x => x.Equals(propertyName)).Count >= 1  && propertyName.Contains("ontology") == true )
                return false;
            return true;
        }
    }
}
