using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Divan.Cloudant.Query
{
    
    public sealed class RequestBuilder
    {
        protected List<string> _keys;
        protected bool _includeDocs;
        protected HttpClient _client;
        protected string _databaseName;
        
        protected RequestBuilder(string databaseName, HttpClient client)
        {
            _client = client;
            _databaseName = databaseName;
            _keys = new List<string>();
            _includeDocs = false;
        }
        public RequestBuilder AddKeys(string[] keys)
        {
            _keys.AddRange(keys);
            return this;
        }

        public RequestBuilder IncludeDocs(bool include = false){
            _includeDocs = include;
            return this;
        }

        public RequestBuilder ClearKeys()
        {
            _keys.Clear();
            return this;
        }

        public AllDocsRequest Build()
        {
            var request = new AllDocsRequest(_databaseName,_client);
            request.IncludeDocs = _includeDocs;
            return request;
        }
    }
}