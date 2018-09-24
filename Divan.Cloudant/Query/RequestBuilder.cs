using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Divan.Cloudant
{
    
    public sealed class RequestBuilder
    {
        private AllDocsParams _allDocsParam;
        
        internal RequestBuilder(string databaseName, HttpClient client)
        {
            _allDocsParam = new AllDocsParams(){
                _client = client,
                _databaseName = databaseName,
                Keys = new List<string>(),
                IncludeDocs = false
            };
        }
        public RequestBuilder AddKeys(string[] keys)
        {
            _allDocsParam.Keys.AddRange(keys);
            return this;
        }

        public RequestBuilder IncludeDocs(bool include = false){
            _allDocsParam.IncludeDocs = include;
            return this;
        }

        public RequestBuilder ClearKeys()
        {
            _allDocsParam.Keys.Clear();
            return this;
        }

        public AllDocsRequest Build()
        {
            var request = new AllDocsRequest(_allDocsParam);
            return request;
        }
    }
}