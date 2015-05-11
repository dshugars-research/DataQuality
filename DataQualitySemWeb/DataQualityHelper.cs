using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualitySemWebLib
{
    public static class DataQualityHelper
    {
        public static string GetMostFrequentResult(List<string> list)
        {
           
            return list
                .GroupBy(i => i)
                .OrderByDescending(g => g.Count())
                .Take(1)
                .Select(g => g.Key)
                .FirstOrDefault();
            
        }
        public static bool ContainsDuplicateResults(List<string> list)
        {
            var res = list
                    .GroupBy(i => i)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Count())
                    .FirstOrDefault();
                    
            if (res > 1)
                return true;
            return false;
        }
    }
}
