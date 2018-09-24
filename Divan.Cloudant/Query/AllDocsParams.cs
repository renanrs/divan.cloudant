using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Divan.Cloudant
{
    internal class AllDocsParams
    {
        public bool IncludeDocs { get; set; }
        public string DataRequest { get; set; }
        public List<string> Keys{get;set;}
        public HttpClient _client;
        public string _databaseName;
    }
}
