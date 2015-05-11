using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualitySemWebLib;

namespace ExamineSameAs
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> uris = new List<string>();
            RandomValidURIGenerator rvURIGen = new RandomValidURIGenerator();
            uris = rvURIGen.Generate(100);
        }
    }
}
