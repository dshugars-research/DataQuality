using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace DataQualitySemWebLib
{
    public class URIValidatorResult
    {
        public SemWebDataQualityViolations Violation = SemWebDataQualityViolations.None;
        public string ValidatedSearch = "";
        public URIValidatorResult(string URL)
        {
            ValidatedSearch = URL;
        }
    }
    public class URIValidator
    {
        private string _origSearch;
        public URIValidatorResult Validate(string search)
        {
            _origSearch = search;
            var res = ValidateIterator(search, 0);
            return res;
        }
        private URIValidatorResult ValidateIterator(string search, int iterationNumber)
        {
            var g = new Graph();
            var results = new URIValidatorResult(search);

            // try the initial search first
            // append .rdf second
            // look for dbpedia specific third
            if (iterationNumber > 100)
            {
                results.Violation = SemWebDataQualityViolations.UnableToDereferenceURI;
                return results; // we dun messed up. is this a violation or just more validation on URIs?
            }

            try
            {
                UriLoader.Load(g, new Uri(search));
                
            }
            catch
            {
                results.Violation = SemWebDataQualityViolations.UnableToDereferenceURI;

                return results;

                if(iterationNumber == 0)
                {
                    iterationNumber++;
                    ValidateIterator(_origSearch + ".rdf", iterationNumber);
                    
                }
                if(iterationNumber==1)
                {
                    iterationNumber++;
                    ValidateIterator("http://dbpedia.org/data/" + _origSearch + ".rdf", iterationNumber);
                }
                if(iterationNumber==2)
                {
                    results.Violation = SemWebDataQualityViolations.UnableToDereferenceURI;

                    return results;
                }
            }
            if (g.IsEmpty)
            {
                results.Violation = SemWebDataQualityViolations.UnableToDereferenceURI;

                return results;
                // let's do some string replacement.  this is very dangerous. what if the uri contains
                // more than one thing to be searched? what if the things aren't in the right position, yada yada
                search = search.Replace("ontology", "data3");
                search = search.Replace("property", "data4");
                search = search + ".rdf";
                iterationNumber++;
                ValidateIterator( search, iterationNumber);
               
                results.ValidatedSearch = search;
            }

            
            return results;
        }
    }
}
