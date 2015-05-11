using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Visualize.Controllers
{

    // is this class going to be used?
    public class URIConstructor
    {
        public string Path {get; set;}
        public URIConstructor(string path)
        {
            Path = path;
        }
    }

    // is this class going to be used?
    public class NodeCreator
    {
        public NodeCreator( string search )
        {

        }
        public NodeCreator( URIConstructor uric)
        {

        }
    }
}