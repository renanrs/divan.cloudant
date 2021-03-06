using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Divan.Cloudant.Exceptions;

namespace Divan.Cloudant
{
    public sealed class AllDocsRequest
    {
        private const string endPoint = "_all_docs";
        internal AllDocsParams _allDocsParams;

        internal AllDocsRequest(AllDocsParams allDocsParams)
        {
            _allDocsParams = allDocsParams;
        }

        private string GetStatement()
        {
            string statement = string.Empty;

            this.GetKeysStatement(ref statement);
            this.GetIncludeDocsStatement(ref statement);
            return statement;
           
        }

        private async Task<string> Request(string statement)
        {
            if(!string.IsNullOrEmpty(statement))
                statement = statement.Insert(0,"?");

            var result = await _allDocsParams._client.GetAsync($"{_allDocsParams._databaseName}/{endPoint}{statement}");
            ExceptionHelper.ThrowDocumentException(result.StatusCode);

            return await result.Content.ReadAsStringAsync();
        }


        private void GetKeysStatement(ref string statement)
        {
            if(!string.IsNullOrEmpty(statement))
                statement += "&";

            if(_allDocsParams.Keys.Count > 0)
                statement = $"keys=[\"{String.Join("","",_allDocsParams.Keys)}\"]";
        }

        private void GetIncludeDocsStatement(ref string statement)
        {
            if(!string.IsNullOrEmpty(statement))
                statement += "&";

            if(_allDocsParams.IncludeDocs)
                statement = $"include_docs=true";
        }

        public AllDocsResponse GetResponse()
        {            
            var jsonResult = Task.Run(async () => await this.Request(this.GetStatement()));

            var jsonString = jsonResult.Result;
            //https://console.bluemix.net/docs/services/Cloudant/api/database.html#get-documents
            return new AllDocsResponse(jsonString);
        }
    }
}