using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Divan.Cloudant.Test
{
    [TestClass]
    public class CloudantClient_Operations
    {
        private const string value = "XUnitTest";
        private readonly CloudantClient _cloudantClient;
        private string docId;

        public CloudantClient_Operations()
        {
            _cloudantClient = new CloudantClient(new CloudantConnection(
                                                    Environment.GetEnvironmentVariable("CLOUDANT_URL"),
                                                    Environment.GetEnvironmentVariable("CLOUDANT_USER"),
                                                    Environment.GetEnvironmentVariable("CLOUDANT_TOKEN")
                                                ));
        }

        
        [TestMethod]
        public void A1_CreateDb()
        {
            try
            {
                var result = Task.Run(() => _cloudantClient.CreateDBAsync(value)).GetAwaiter();
                result.GetResult();
                //Assert.IsTrue(result.GetResult() == System.Net.HttpStatusCode.Created, $"Database was not created: {result.GetResult()}");
                Thread.Sleep(2000);
            }
            catch (Divan.Cloudant.Exceptions.PreconditionFailedException ex)
            {                
                Assert.Fail($"Database was not created, PreconditionFailedException has been thrown",ex);
            }
            catch(System.Exception e)
            {
                Assert.Fail($"Database was not created, GenericException has been thrown",e);
            }
            
        }

        [TestMethod]
        public void A2_GetAllDbs()
        {
            var result = Task.Run(() => _cloudantClient.GetAllDbsNameAsync()).GetAwaiter();
            Assert.IsTrue(result.GetResult().FindAll(x => x.Equals(value.ToLower())).Count > 0);
            Thread.Sleep(2000);
        }

        [TestMethod]
        public void A3_GetDatabase()
        {
            var result = Task.Run(() => _cloudantClient.GetDatabaseAsync(value)).GetAwaiter();
            Assert.IsTrue(result.GetResult() != null , $"Database was not caught: {result.GetResult().DatabaseName}");
            Thread.Sleep(2000);
        }

        [TestMethod]
        public void A4_CreateDoc()
        {
            var obj = CreateDoc();
            
            Assert.IsTrue(obj != null, $"Document was created: {obj.id}");
            if (obj != null)
            {
                docId = obj.id;
            }
            Thread.Sleep(2000);
        }

        [TestMethod]
        public void A5_FindDoc()
        {
            var result = Task.Run(() => _cloudantClient.GetDatabaseAsync(value)).GetAwaiter();
            docId = CreateDoc().id;

            var db = result.GetResult();
            
            var findReturn = Task.Run(() => db.FindAsync<CepModel>(docId)).GetAwaiter();
            var obj = findReturn.GetResult();
            
            Assert.IsTrue(obj != null, $"Document was retrieved: {obj.Cep}");
            //Assert.IsTrue(obj.Cep.Equals("18103610"), "Cep didn't expected");
            Thread.Sleep(2000);

        }

        [TestMethod]
        public void A6_BulkOfDocs()
        {
            var result = Task.Run(() => _cloudantClient.GetDatabaseAsync(value)).GetAwaiter();

            var db = result.GetResult();
            var doc = CepModel.GetInstancesCreateBulk();

            var saveReturn = Task.Run(() => db.BulkOfDocsAsync<CepModel>(doc)).GetAwaiter();
            var obj = saveReturn.GetResult();
            var objList = new List<ResultObject>(obj);
            Assert.IsTrue(objList.Count == 4, $"There are documents not accepted: {objList.Count}");
            Assert.IsTrue(objList.Exists(p => p.id.Equals("99999999999999")), $"There is document not created: {objList.Count}");
        }

        [TestMethod]
        public void A6_BulkOfDocsUpdate()
        {
            var result = Task.Run(() => _cloudantClient.GetDatabaseAsync(value)).GetAwaiter();

            var db = result.GetResult();
            var doc = new List<CepModel>(CepModel.GetInstancesCreateBulk());
            var findResult = Task.Run(() => db.FindAsync<CepModel>("99999999999999")).GetAwaiter();
            var objFind = findResult.GetResult();

            doc.ForEach((p) => {
                if (!string.IsNullOrEmpty(p._id) && p._id.Equals("99999999999999"))
                {
                    p._rev = objFind._rev;
                    p.Logradouro = "xxxXxxxxXXxxx";
                }
            });

            var saveReturn = Task.Run(() => db.BulkOfDocsAsync<CepModel>(doc)).GetAwaiter();
            var obj = saveReturn.GetResult();
            var objList = new List<ResultObject>(obj);
            Assert.IsTrue(objList.Count == 4, $"There are documents not accepted: {objList.Count}");
            Assert.IsTrue(objList.Exists(p => p.id.Equals("99999999999999") && p.ok == "true"), $"There is document not created: {objList.Count}");

            findResult = Task.Run(() => db.FindAsync<CepModel>("99999999999999")).GetAwaiter();
            objFind = findResult.GetResult();

            Assert.IsNotNull(objFind, "Updated document not found.");
            Assert.IsTrue(objFind.Logradouro.Equals("xxxXxxxxXXxxx"), "Document id 99999999999999 was not updated.");
        }

        [TestMethod]
        public void A6_GetAllDocs(){

            try
            {
                var result = Task.Run(() => _cloudantClient.GetDatabaseAsync(value)).GetAwaiter();
                var db = result.GetResult();

                var allDocsReturn = Task.Run(() => db.GetAllDocs()).GetAwaiter();
                var obj = allDocsReturn.GetResult();

                Assert.IsFalse(string.IsNullOrEmpty(obj),"Something wrong happend here, GetAllDocs operation");
            }
            catch (System.Exception e)
            {
                
                 Assert.Fail($"All docs weren't retrieved,an Exception has been thrown",e);
            }
        }

        [TestMethod]
        public void A6_GetAllDocsIncludeDocks(){

            try
            {
                var result = Task.Run(() => _cloudantClient.GetDatabaseAsync(value)).GetAwaiter();
                var db = result.GetResult();

                var allDocsReturn = Task.Run(() => db.GetAllDocs("")).GetAwaiter();
                var obj = allDocsReturn.GetResult();
                var AllRequest=obj.IncludeDocs(true).Build();
                var json = AllRequest.GetResponse();
                var docs = json.GetDocsAs<CepModel>();
                Assert.IsTrue(docs != null,"Something wrong happend here, GetAllDocs operation");
                Assert.IsTrue(docs.Count > 0,"There is no data got by the method");
                Assert.IsInstanceOfType(docs,typeof(List<CepModel>),"Json was converted to wrong type");
            }
            catch (System.Exception e)
            {
                
                 Assert.Fail($"All docs weren't retrieved,an Exception has been thrown",e);
            }
        }

        


        [TestMethod]
        public void A6_UpdateDoc()
        {
            var result = Task.Run(() => _cloudantClient.GetDatabaseAsync(value)).GetAwaiter();
            var objId = CreateDoc().id;
            var db = result.GetResult();
            var findResult = Task.Run(() => db.FindAsync<CepModel>(objId)).GetAwaiter();

            var document = findResult.GetResult();

            document.Logradouro = "Rua Antonio Joaquim Santana";

            var updateResult = Task.Run(() => db.UpdateAsync<CepModel>(document)).GetAwaiter();
            var obj = updateResult.GetResult();

            Assert.IsTrue(obj != null, $"Document was not updated");

            Thread.Sleep(2000);

        }

        [TestMethod]
        public void A7_DeleteDoc()
        {
            try
            {
                var result = Task.Run(() => _cloudantClient.GetDatabaseAsync(value)).GetAwaiter();

                var db = result.GetResult();
                var docResult = CreateDoc();

                var deleteResult = Task.Run(() => db.DeleteAsync(docResult.id,docResult.rev)).GetAwaiter();
                var obj = deleteResult.GetResult();
                Assert.IsTrue(bool.Parse(obj.ok),$"Document wasn't deleted {obj.error}");

            }catch(System.Exception e){

                Assert.Fail($"Database was not deleted, Exception has been thrown",e);
            }

            Thread.Sleep(2000);
        }

        [TestMethod]
        public void A8_DeleteDb()
        {
            try
            {
                var result = Task.Run(() => _cloudantClient.DeleteDBAsync(value)).GetAwaiter();
                result.GetResult();
                
                Thread.Sleep(2000);
            }
            catch (Divan.Cloudant.Exceptions.DatabaseNotFoundException ex)
            {
                Assert.Fail("DatabaseNotFoundException has been thrown.",ex);
            }
            
        }

        private ResultObject CreateDoc()
        {
            var result = Task.Run(() => _cloudantClient.GetDatabaseAsync(value)).GetAwaiter();

            var db = result.GetResult();
            var doc = CepModel.GetInstance();

            var saveReturn = Task.Run(() => db.CreateAsync<CepModel>(doc)).GetAwaiter();
            var obj = saveReturn.GetResult();

            return obj;
        }
    }

    public class CepModel    {

        public string _id { get; set; }
        public string _rev { get; set; }
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Bairro { get; set; }

        public static CepModel GetInstance()
        {
            return new CepModel
            {
                _id = null,
                _rev = null,
                Cep = "18103610",
                Bairro = "Jardim Primavera",
                Logradouro = "Rua AngeloZanardo"
            };
        }

        public static IEnumerable<CepModel> GetInstancesCreateBulk()
        {
            return new List<CepModel> {
                new CepModel
                {
                    _id = null,
                    _rev = null,
                    Cep = "18090360",
                    Bairro = "Vila Progresso",
                    Logradouro = "Rua Jo�o Cordeiro"
                },
                 new CepModel
                {
                    _id = null,
                    _rev = null,
                    Cep = "18103610",
                    Bairro = "Jardim Primavera",
                    Logradouro = "Rua Angelo Zanardo"
                },
                  new CepModel
                {
                    _id = null,
                    _rev = null,
                    Cep = "18090230",
                    Bairro = "Maria do Carmo",
                    Logradouro = "Rua Antonio J Santana"
                },
                   new CepModel
                {
                    _id = "99999999999999",
                    _rev = null,
                    Cep = "18080751",
                    Bairro = "Retiro S�o Jo�o",
                    Logradouro = "Rua Alceste del Cistia"
                },
            };
        }


        public static CepModel GetUpdateInstance()
        {
            return new CepModel
            {
                _id = "123456",
                _rev = string.Empty,
                Cep = "18103610",
                Bairro = "Jardim PrimaEden",
                Logradouro = "Rua Angelino Zanardo"
            };
        }
    }
}
