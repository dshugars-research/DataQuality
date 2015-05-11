using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;
using System.Net;
using System.IO;

namespace DataQualitySemWebLib
{
    public class RandomValidURIGenerator
    {
        public List<string> Generate(int numberToGenerate)
        {
            int counterForRandomURL = 0;
            List<string> uris = new List<string>();
            int numberOfRandomURL = numberToGenerate;
            while (counterForRandomURL < numberOfRandomURL) // counter is less than number of random url to generate 
            {
                counterForRandomURL++;
                while (true)
                {
                    string url = generateRandomURL();
                    if (!string.IsNullOrEmpty(url))
                    {
                        IGraph g = new Graph();

                        UriLoader.Load(g, new Uri(url));
                        string[] temp = new string[5];
                        try
                        {
                            if (g.Triples.Count > 1)  //to check if the url is valid
                            {
                                uris.Add(url);
                                break;
                            }
                        }
                        catch (Exception)
                        {
                        }

                    }
                }
            }
            return uris;
        }

        private string generateRandomURL()
        {
            string sURL = @"http://randomword.setgetgo.com/get.php";
            WebRequest wrGETURL = WebRequest.Create(sURL);
            WebProxy myProxy = new WebProxy("myproxy", 80);
            myProxy.BypassProxyOnLocal = true;
            wrGETURL.Proxy = WebProxy.GetDefaultProxy();
            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();
            StreamReader objReader = new StreamReader(objStream);
            string sLine = "";
            int i = 0;
            string url = "";
            while (sLine != null)
            {
                i++;
                sLine = objReader.ReadLine();
                if (!string.IsNullOrEmpty(sLine))
                    url = @"http://dbpedia.org/resource/" + sLine;
            }
            return url;
        }
    }
}
