using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace DataQualitySemWebLib
{
    public class SemWebDataSuggestionGeneratorResult
    {
        public string Source = "";
        public string Value = "";
    }
    public class SemWebDataSuggestionGenerator
    {
        public List<string> Keywords = new List<string>();
        public SemWebDataSuggestionGeneratorResult Result = new SemWebDataSuggestionGeneratorResult();

        public SemWebDataSuggestionGenerator(string URI)
        {
           Result = SearchGoogleCacheByURI(URI);  
            //string SwoogleResult = SearchSwoogleCacheByURI;
            //ExtractURIKeywords(URI);
            
        }
        private string FormatGoogleURL(string URI)
        {
            return "http://webcache.googleusercontent.com/search?q=cache:" + URI;
        }
        private SemWebDataSuggestionGeneratorResult SearchGoogleCacheByURI(string passedURI)
        {
            var result = new SemWebDataSuggestionGeneratorResult();

            var URI = FormatGoogleURL(passedURI);
            result.Source = URI;
            // load the graph
            // use the value extractor
            var g = new Graph();
            // temp
            try
            {
                UriLoader.Load(g, new Uri(URI));
            }
            catch(Exception e)
            {
                return result;
            }
            

            var gt = new GraphTranslator(g);
            var lg = gt.GetTranslation();
            var pn = new PropertyNameExtractor(lg);
            var str = pn.FindBasicLabelURI();
            var labelURI = str.Result;
            var pv = new PropertyValueExtractor(lg);
            pv.GetResults(str.Result);

            var valueRes = pv.GetResults(labelURI);
            result.Value = valueRes.Results[0];
            return result;

        }
        public void ProcessKeywords()
        {
            ProcessUsingSwoogle();
        }
        private string ProcessUsingSwoogle()
        {
            var URI = FormatSwoogleURL();
            // load the graph
            // use the value extractor
            var g = new Graph();
            // temp
            URI = "http://webcache.googleusercontent.com/search?q=cache:http://dbpedia.org/ontology/designer";
            UriLoader.Load(g, new Uri(URI));

            var gt = new GraphTranslator(g);
            var lg = gt.GetTranslation();
            var pn = new PropertyNameExtractor(lg);
            var str = pn.FindBasicLabelURI();
            var pv = new PropertyValueExtractor(lg);
            var valueRes = pv.GetResults(str.Result);
            return valueRes.Results[0];

        }
        private string FormatSwoogleURL()
        {
            return "http://swoogle.umbc.edu/index.php?option=com_frontpage&service=digest&queryType=digest_swt&searchString=http%3A%2F%2Fdbpedia.org%2Fontology%2Fdesigner&view=raw";
        }
        private void ExtractURIKeywords(string URI)
        {
            Uri uri = UriFactory.Create(URI);
            foreach( string u in uri.Segments )
            {
                Keywords.Add(u);
            }
        }
    }
}
