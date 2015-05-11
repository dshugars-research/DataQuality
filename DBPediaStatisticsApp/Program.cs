using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;
using DataQualitySemWebLib;
using System.Data.SqlClient;

namespace DBPediaStatisticsApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var g = new Graph();
            const int MaxURICount = 5000;
            //string URI = "http://dbpedia.org/resource/Tetris";
            //string URI = "http://dbpedia.org/resource/Tetris_Mania";
            //string URI = "http://dbpedia.org/resource/Michael_Jackson";
            var list = new List<string>();
            list.Add("http://dbpedia.org/resource/Detroit");
            list.Add("http://dbpedia.org/resource/HTML");
            list.Add("http://dbpedia.org/resource/George_Washington");
            list.Add("http://dbpedia.org/resource/Apple");
            list.Add("http://dbpedia.org/resource/Microsoft");
            list.Add("http://dbpedia.org/resource/John_Travolta");
            list.Add("http://dbpedia.org/resource/Catholic_Church");
            list.Add("http://dbpedia.org/resource/Film");
            list.Add("http://dbpedia.org/page/Billy_Joel");
            list.Add("http://dbpedia.org/page/Magic_Johnson");

            SqlConnection myConnection = new SqlConnection("user id=Visualize;" +
                           "password=Visualiz3;server=pc6eraqyib.database.windows.net,1433;" +
                           "Trusted_Connection=No;" +
                           "database=Visualization; " +
                           "connection timeout=30");
            try
            {
                myConnection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            foreach( var el in list)
            {
                string URI = el;
                g = new Graph();
                UriLoader.Load(g, new Uri(URI));

                // log this attempt at this URI
                bool success = LogAttempt(URI, g, myConnection, el);

                if (success)
                {
                    // we successfully parsed, log all available child URIs but don't parse
                    RecordChildURIs(URI, g, myConnection, el);
                }

                // find the next URI to attempt to parse. this will need to end up being a loop
                URI = GetNextURI(myConnection);
                while (URI.Length > 0)
                {
                    g = new Graph();
                    try
                    {
                        UriLoader.Load(g, new Uri(URI));
                    }
                    catch(Exception e)
                    {
                        g = null;
                        Console.WriteLine(e.ToString());
                    }
                
                    success = LogAttempt(URI, g, myConnection, el);
                    int URIRecordCount = GetURIRecordCount(myConnection, el);
                    if( success && URIRecordCount <= MaxURICount)
                    {
                       RecordChildURIs(URI, g, myConnection, el);
                    }
                    URI = GetNextURI(myConnection);                    
                }
            }
        }

        private static int GetURIRecordCount(SqlConnection myConnection, string ParentURI)
        {
            SqlCommand myCommand = new SqlCommand();
            int RecordCount=0;
            myCommand.Connection = myConnection;
            myCommand.CommandText = "select count(*) from URIStats where ParentURI = '" + ParentURI.Replace("'", "''") + "'";
            SqlDataReader myReader = null;
            myReader = myCommand.ExecuteReader();
            if (myReader.HasRows)
            {
                myReader.Read();
                RecordCount = myReader.GetInt32(0);
            }
            myReader.Close();
            return RecordCount;
        }
        private static string GetNextURI(SqlConnection myConnection)
        {
            SqlCommand myCommand = new SqlCommand();
            string nextURI = "";
            myCommand.Connection = myConnection;
            myCommand.CommandText = "select top 1 URI from URIStats where Parsed IS NULL ";
            SqlDataReader myReader = null;
            myReader = myCommand.ExecuteReader();
            if( myReader.HasRows)
            {
                myReader.Read();
                nextURI = myReader.GetString(0);
            }
            myReader.Close();
            return nextURI;
        }

        private static void RecordChildURIs(string URI, Graph g, SqlConnection myConnection, string ParentURI)
        {
            foreach (var x in g.Triples)
            {
                if ( x.Subject.NodeType == NodeType.Uri )
                {
                    LogURIToParse(x.Subject.ToString(), myConnection, ParentURI);
                }

                if(x.Predicate.NodeType == NodeType.Uri)
                {
                    LogURIToParse(x.Predicate.ToString(), myConnection, ParentURI);
                }
            }
            SqlCommand myCommand = new SqlCommand();
            myCommand.Connection = myConnection;
            myCommand.CommandText = "update URIStats set AttemptedParseChildNodes = 1 where URI = '" +  URI.Replace("'", "''") + "'";
            
            Console.WriteLine(myCommand.CommandText);

            myCommand.ExecuteNonQuery();
        }

        private static void LogURIToParse(string URI, SqlConnection myConnection, string ParentURI)
        {
            SqlCommand myCommand = new SqlCommand();
            myCommand.Connection = myConnection;
            
            myCommand.CommandText = "select * from URIStats where URI = '" + URI.Replace("'", "''") + "'";
            SqlDataReader myReader = null;
            myReader = myCommand.ExecuteReader();
            bool recsFound = myReader.HasRows;
            myReader.Close();

            if (recsFound == false)
            {
                myReader.Close();

                myCommand.CommandText = myCommand.CommandText = "insert into URIStats (URI, ParentURI) Values ( '" + URI.Replace("'", "''") + "', '" + ParentURI.Replace("'", "''") + "') ";
                Console.WriteLine(myCommand.CommandText);
                myCommand.ExecuteNonQuery();
            }

        }

        private static bool LogAttempt(string URI, Graph g, SqlConnection myConnection, string ParentURI)
        {
            bool graphLoaded = true;
            SqlCommand myCommand = new SqlCommand();
            myCommand.Connection = myConnection;
            myCommand.CommandText = "select * from URIStats where URI = '" + URI.Replace("'", "''") + "'";
            SqlDataReader myReader = null;
            myReader = myCommand.ExecuteReader();
            bool recsFound = myReader.HasRows;
            myReader.Close();

            if( g == null )
            {
                graphLoaded = false;
            }
            else
            {
                if( g.IsEmpty )
                    graphLoaded = false;
            }

            if( recsFound == false )
            {

                myCommand.CommandText = "insert into URIStats (URI, Parsed, ParentURI) Values ( '" + URI.Replace("'", "''") + "', " + Convert.ToInt32(graphLoaded) + ", '" + ParentURI.Replace("'", "''") + "') ";
                
            }
            else
            {
                myCommand.CommandText = "update URIStats set Parsed = " + Convert.ToInt32(graphLoaded) + " where URI = '" + URI.Replace("'", "''") + "' ";
            }
            Console.WriteLine(myCommand.CommandText);

            myCommand.ExecuteNonQuery();
            return graphLoaded;
        }
    }
}
