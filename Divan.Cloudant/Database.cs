using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Divan.Cloudant.Exceptions;
using Divan.Cloudant;

namespace Divan.Cloudant
{
    public sealed class Database 
    {
        public string DatabaseName { get; private set; }
        private HttpClient _client;
        private const string mediaType = "application/json";

        internal Database(string databaseName, HttpClient client)
        {
            _client = client;
            DatabaseName = databaseName;
        }
        /// <summary>
        /// Method creates a new document based on an object.
        /// </summary>
        /// <typeparam name="ResultObject">Result from database</typeparam>
        public async Task<ResultObject> CreateAsync<T>(T doc) where T : class
        {
            var docJson = JsonSerializer.Serialize<T>(doc);
            docJson = docJson.Replace(",\"_rev\":null", string.Empty)
                                 .Replace("\"_rev\":null,", string.Empty)
                                 .Replace(",\"_rev\":\"\"", string.Empty)
                                 .Replace("\"_rev\":\"\",", string.Empty);

            var propId = typeof(T).GetProperty("_id");
            if (propId != null)
            {
                var id = propId.GetValue(doc) as string;
                if (string.IsNullOrEmpty(id))
                {
                    docJson = docJson.Replace(",\"_id\":null", string.Empty)
                                    .Replace("\"_id\":null,", string.Empty)
                                    .Replace(",\"_id\":\"\"", string.Empty)
                                    .Replace("\"_id\":\"\",", string.Empty);
                }
            }

            var result = await _client.PostAsync(DatabaseName, new StringContent(docJson, Encoding.UTF8, mediaType));

            ExceptionHelper.ThrowDocumentException(result.StatusCode);
           
            var jsonResult = await result.Content.ReadAsStringAsync();
            var obj = JsonSerializer.Deserialize<ResultObject>(jsonResult) as ResultObject;

            return obj;
        }

        /// <summary>
        /// This method create or updates a bulk of documents.
        /// </summary>
        /// <typeparam name="ResultObject">Result from database</typeparam>
        public async Task<IEnumerable<ResultObject>> BulkOfDocsAsync<T>(IEnumerable<T> doc) where T : class
        {
            var docJson = JsonSerializer.Serialize(new Bulk<T>(doc));
            docJson = docJson.Replace(",\"_rev\":null", string.Empty)
                                 .Replace("\"_rev\":null,", string.Empty)
                                 .Replace(",\"_rev\":\"\"", string.Empty)
                                 .Replace("\"_rev\":\"\",", string.Empty);

            var propId = typeof(T).GetProperty("_id");
            if (propId != null)
            {                
                docJson = docJson.Replace(",\"_id\":null", string.Empty)
                                .Replace("\"_id\":null,", string.Empty)
                                .Replace(",\"_id\":\"\"", string.Empty)
                                .Replace("\"_id\":\"\",", string.Empty);
                
            }

            var result = await _client.PostAsync($"{DatabaseName}/_bulk_docs", new StringContent(docJson, Encoding.UTF8, mediaType));
            
            ExceptionHelper.ThrowDocumentException(result.StatusCode);
           
            var jsonResult = await result.Content.ReadAsStringAsync();
            var obj = JsonSerializer.Deserialize<IEnumerable<ResultObject>>(jsonResult) as IEnumerable<ResultObject>;

            return obj;
        }

        /// <summary>
        /// This method updates a document using its id to find it.
        /// </summary>
        /// <param name="doc">Generic object that will be converted to json format</param>
        /// <param name="id">Document's id</param>
        /// <typeparam name="ResultObject">Resul from database</typeparam>
        public async Task<ResultObject> UpdateAsync<T>(T doc, string id) where T : class
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                throw new NullReferenceException("The id parameter can not be null");

            var docJson = JsonSerializer.Serialize<T>(doc);
            var result = await _client.PutAsync($"{DatabaseName}/{id}", new StringContent(docJson, Encoding.UTF8, mediaType));

            ExceptionHelper.ThrowDocumentException(result.StatusCode);
            
            var jsonResult = await result.Content.ReadAsStringAsync();
            var obj = JsonSerializer.Deserialize<ResultObject>(jsonResult) as ResultObject;

            return obj;
        }

        /// <summary>
        /// This method updates a document using its id to find it. 
        /// The document object must have a public _id property.
        /// </summary>
        /// <param name="doc">Generic object that will be converted to json format</param>
        /// <typeparam name="ResultObject">Resul from database</typeparam>
        public async Task<ResultObject> UpdateAsync<T>(T doc) where T : class
        {
            
            var propId = typeof(T).GetProperty("_id");
            if (propId != null)
            {
                var docJson = JsonSerializer.Serialize<T>(doc);
                var id = propId.GetValue(doc);                

                var result = await _client.PutAsync($"{DatabaseName}/{id}", new StringContent(docJson, Encoding.UTF8, mediaType));

                ExceptionHelper.ThrowDocumentException(result.StatusCode);

                var jsonResult = await result.Content.ReadAsStringAsync();
                var obj = JsonSerializer.Deserialize<ResultObject>(jsonResult) as ResultObject;
                return obj;
            }
            else
            {
                throw new Exception("_id not found");
            }
            
        }

        /// <summary>
        /// This method retrieves a document object based on id param.
        /// </summary>
        /// <param name="id">Document's id</param>
        /// <typeparam name="T">Document object</typeparam>
        public async Task<T> FindAsync<T>(string id) where T : class
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                throw new NullReferenceException("The id parameter can not be null");

            var result = await _client.GetAsync($"{DatabaseName}/{id}");

            ExceptionHelper.ThrowDocumentException(result.StatusCode);
        
            var jsonResult = await result.Content.ReadAsStringAsync();
            var obj = JsonSerializer.Deserialize<T>(jsonResult) as T;

            return obj;
        }

        /// <summary>
        /// This method retrieves a document object based on id of specific revision.
        /// </summary>
        /// <param name="id">Document's id</param>
        /// <param name="rev">Document's revision number</param>
        /// <typeparam name="T">Document object</typeparam>
        public async Task<T> FindAsync<T>(string id, string rev) where T : class
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                throw new NullReferenceException("The id parameter can not be null");

            if (string.IsNullOrEmpty(rev) || string.IsNullOrWhiteSpace(rev))
                throw new NullReferenceException("The rev parameter can not be null");

            return await FindAsync<T>($"{id}?rev={rev}");
        }

        public async Task<ResultObject> DeleteAsync(string id, string rev) 
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                throw new NullReferenceException("The id parameter can not be null");

            if (string.IsNullOrEmpty(rev) || string.IsNullOrWhiteSpace(rev))
                throw new NullReferenceException("The rev parameter can not be null");

            var result = await _client.DeleteAsync($"{DatabaseName}/{id}?rev={rev}");

            ExceptionHelper.ThrowDocumentException(result.StatusCode);
           
            var jsonResult = await result.Content.ReadAsStringAsync();
            var obj = JsonSerializer.Deserialize<ResultObject>(jsonResult) as ResultObject;

            return obj;
        }

        public async Task<string> GetAllDocs(){

            var result = await _client.GetAsync($"{DatabaseName}/_all_docs");
            ExceptionHelper.ThrowDocumentException(result.StatusCode);

            var jsonResult = result.Content.ReadAsStringAsync();

            return jsonResult.Result;
        }

        public async Task<RequestBuilder> GetAllDocs(string test)
        {            
            return new RequestBuilder(DatabaseName,_client);
        }

        
    }
}
