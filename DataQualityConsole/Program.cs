using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;
using DataQualitySemWebLib;
using System.Data;
using System.Threading;
using System.Net;

namespace DataQualityConsole
{

    class Program
    {
        static void Main(string[] args)
        {
            var suggestion = new SemWebDataSuggestionGenerator("http://dbpedia.org/ontology/wikiPageRedirects");
            suggestion.ProcessKeywords();

            //Interesting: http://lookup.dbpedia.org/api/search.asmx/KeywordSearch?QueryString=Kindle

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://dbpedia.org/ontology/wikiPageRedirects");
            request.ContentType = "application/rdf+xml; charset=UTF-8";
            request.Method = "POST";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            foreach(var el in response.Headers.AllKeys)
            {
                System.Console.WriteLine(el + ":");
                System.Console.WriteLine(response.Headers[el]);
                System.Console.WriteLine("------------------------------------------------");
            }
            System.Console.Write(response.ResponseUri);

            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            var uriVal = new URIValidator();
            //uriVal.Validate("http://dbpedia.org/property/hasPhotoCollection");
            uriVal.Validate("http://dbpedia.org/resource/Tetris");
            
            var g = new Graph();

            UriLoader.Load(g, new Uri("http://dbpedia.org/resource/Tetris"));

            foreach(var x in g.Triples)
            {
                if( x.Object.ToString().Contains("Alexey")  )
                {
                    Console.WriteLine("Looking for Alexey");
                }
            }

            UriLoader.Load(g, new Uri("http://dbpedia.org/ontology/wikiPageRedirects"), new RdfXmlParser());

            //Get all Triples which meet some criteria
            //Want to find everything that is rdf:type
            // IUriNode rdfType = g.CreateUriNode("rdfs:label");
        
            // var ts = g.GetTriplesWithPredicate(rdfType);


            // DataTable dt = g.ToDataTable();
             
            var gt = new GraphTranslator(g);
            var lg = gt.GetTranslation();
            var pn = new PropertyNameExtractor(lg);
            var str = pn.FindBasicLabelURI();
            var pv = new PropertyValueExtractor(lg);
            pv.GetResults(str.Result);

        }
    }
}
