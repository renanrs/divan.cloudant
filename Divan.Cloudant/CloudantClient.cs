using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Divan.Cloudant.Exceptions;

namespace Divan.Cloudant
{
    public sealed class CloudantClient
    {
        private HttpClientHandler _handler;
        private HttpClient _client;
        private CloudantConnection _connection;

        public CloudantClient(CloudantConnection connection)
        {            
            _connection = connection;
            _handler = new HttpClientHandler();
            _handler.Credentials = connection.Credential;
            _client = new HttpClient(_handler)
            {
                BaseAddress = connection.Uri
            };

            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
        
        /// <summary>
        /// Method creates a database with the database name inputed
        /// </summary>
        /// <param name="databaseName">Database Name</param>
        public async Task CreateDBAsync(string databaseName)
        {
            var response = await _client.PutAsync(databaseName.ToLower(), null);
            ExceptionHelper.ThrowDbException(response.StatusCode);

        }

        /// <summary>
        /// Method deletes a database
        /// </summary>
        /// <param name="databaseName">Database Name</param>
        public async Task DeleteDBAsync(string databaseName)
        {
            databaseName = databaseName.ToLower();
            var dbList = await GetAllDbsNameAsync();
            if (dbList.FirstOrDefault(x => x == databaseName) == null)
                throw new DatabaseNotFoundException($"{databaseName} not found.");

            await _client.DeleteAsync(databaseName); 
        }

        /// <summary>
        /// Method retrieves all database's name
        /// </summary>
        public async Task<List<string>> GetAllDbsNameAsync()
        {
            var response = await _client.GetAsync("_all_dbs");

            ExceptionHelper.ThrowDbException(response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<string>>(json) as List<string>;
        }

        //Future implementation
        // public async Task<List<Database>> GetAllDbsAsync()
        // {
        //     var list = await GetAllDbsNameAsync();
        //     var databases = new List<Database>();

        //     list.ForEach(p=> {
        //         databases.Add(new Database(p,_client));
        //     });

        //     return databases;
        // }

        /// <summary>
        /// Method retrieves an specific database instance by database name or create one
        /// </summary>
        /// <param name="databaseName">Database Name</param>
        /// <param name="create">Informing to create the database in case it does not exist</param>
        public async Task<Database> GetDatabaseAsync(string databaseName,bool create = false)
        {
            databaseName = databaseName.ToLower();

            var dbList = await GetAllDbsNameAsync();
            if (dbList.FirstOrDefault(x => x == databaseName) == null){
                if(create)
                    await CreateDBAsync(databaseName);
                else
                    throw new DatabaseNotFoundException($"{databaseName} not found.");
            }

            return new Database(databaseName, _client);
        }

        
    }
}
