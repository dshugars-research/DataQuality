using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace DataQualitySemWebLib
{
    public class PropertyNameExtractorResult
    {
        public SemWebDataQualityViolations Violation;
        public string Result;
        public PropertyNameExtractorResult()
        {
            // initialize to no violations
            Violation = SemWebDataQualityViolations.None;
        }
    }
    public class PropertyNameExtractor
    {
        private List<FlatPropertyValueEntries> _entries;
        public PropertyNameExtractor(List<FlatPropertyValueEntries> entries)
        {
            _entries = entries;
        }
        // returns a string for the basic label from the entries
        public PropertyNameExtractorResult FindBasicLabelURI()
        {

            var results = new List<string>();
            var pr = new PropertyNameExtractorResult();

            foreach (var el in _entries)
            {
                if (el.PropertyName.Contains("label"))
                {
                    results.Add(el.PropertyName);
                }
            }
            if (results.Count > 1)
            {
                pr.Violation = SemWebDataQualityViolations.MultiplePropertiesDefined;
                pr.Result = DataQualityHelper.GetMostFrequentResult(results);
                return pr;
            }
            pr.Result = results.FirstOrDefault();
            return pr;
        }
    }
}
