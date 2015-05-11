using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualitySemWebLib
{
    public class PropertyValueExtractorResult
    {
        public SemWebDataQualityViolations Violation;
        public List<string> Results;
        public PropertyValueExtractorResult()
        {
            // initialize to no violations
            Violation = SemWebDataQualityViolations.None;
            Results = new List<string>();
        }
    }
    public class PropertyValueExtractor
    {
        private List<FlatPropertyValueEntries> _entries;

        public PropertyValueExtractor(List<FlatPropertyValueEntries> entries)
        {
            _entries = entries;
        }
        public PropertyValueExtractorResult GetResults(string propertyURI)
        {
            var res = new PropertyValueExtractorResult();

            var results = _entries
                        .Where(x => x.PropertyName == propertyURI)
                        .Select(x=>x.Values);

            var stringValues = new List<string>();
            
            foreach( var el in results)
            {
                foreach (var x in el)
                {
                    if (x.Value.IndexOf("@") == x.Value.Length - 3)
                    {
                        x.Value = x.Value.Substring(0, x.Value.Length - 3);
                    }
                        res.Results.Add(x.Value);
                    
                }
            }

            if(DataQualityHelper.ContainsDuplicateResults(res.Results))
                res.Violation = SemWebDataQualityViolations.MultipleValuesFound;
            DataQualityHelper.GetMostFrequentResult(res.Results);
            return res;
        }

    }
}
