using System;
using System.Net;
using System.Net.Http;

namespace Divan.Cloudant.Query
{
    public class AllDocsRequest
    {
        internal bool IncludeDocs { get; set; }
        internal string DataRequest { get; set; }
        private string _databaseName {get;set;}
        private HttpClient _client{get;set;}

        internal AllDocsRequest(string databaseName, HttpClient client)
        {
            _databaseName = databaseName;
            _client = client;
        }
        public AllDocsResponse GetResponse()
        {
            return new AllDocsResponse();
        }
    }
}