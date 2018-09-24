using System;
using System.Linq;
using System.Collections.Generic;
namespace Divan.Cloudant
{
    
    public sealed class AllDocsResponse
    {
        private object _viewResponse;
        private string _json;
        internal AllDocsResponse(string jsonResult )
        {
           _json = jsonResult;
        }

        public List<T> GetDocsAs<T>() where T : class
        {
            if(_viewResponse == null)
                _viewResponse = JsonSerializer.Deserialize<AllDocsViewResponse<T>>(_json);

            var docList = (from p in ((AllDocsViewResponse<T>)_viewResponse).rows select p.doc);
            return docList.ToList();
        }
        
    }
}