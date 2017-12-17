using IrrigationWorker.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace IrrigationWorker.Services
{
    public class DAL
    {
        private DocumentCollection _collection;
        public DAL()
        {
           
        }
        #region sql
         private readonly string connString = "Server=tcp:irrigationserver.database.windows.net,1433;Initial Catalog=Irrigation;Persist Security Info=False;User ID=aprodaniuc;Password=P@ssw0rd123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public void SaveWeatherInfo(string info)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using(SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = string.Format("Insert into WeatherInfo(Info) values('{0}')", info);
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }



        #endregion

        #region docdb
        public async Task SaveWeatherInfoDocDb(string info)
        {
            var endpoint = ConfigurationManager.AppSettings["DocDbEndpoint"];
            var masterKey = ConfigurationManager.AppSettings["DocDbMasterKey"];
            using (var client = new DocumentClient(new Uri(endpoint), masterKey))
            {
                Database database = client.CreateDatabaseQuery("SELECT * FROM c WHERE c.id = 'irrigation'").AsEnumerable().First();
                _collection = client.CreateDocumentCollectionQuery(database.CollectionsLink, "SELECT * FROM c WHERE c.id = 'irrigationCollection'").AsEnumerable().First();
                var doc = await CreateDocument(client, info);
            }
        }

        private async Task<Document> CreateDocument(DocumentClient client, string documentJson)
        {
            var documentObject = new JavaScriptSerializer().Deserialize<object>(documentJson);
            return await CreateDocument(client, documentObject);
        }

        private async Task<Document> CreateDocument(DocumentClient client, object documentObject)
        {
            try
            {
                var result = await client.CreateDocumentAsync(_collection.SelfLink, documentObject);
                var document = result.Resource;
                Console.WriteLine("Created new document: {0}\r\n{1}", document.Id, document);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        #endregion
    }
}
