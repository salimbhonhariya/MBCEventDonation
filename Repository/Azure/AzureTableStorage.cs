using MBCEventDonation.Repository.EventDonation;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MBCEventDonation.Repository.Azure
{
    public class AzureTableStorage<T> : IAzureTableStorage<T> where T : TableEntity, new()
    {
        private readonly AzureTableSettings settings;
        public AzureTableStorage(AzureTableSettings settings)
        {
            this.settings = settings;
            this.settings = settings;
        }
        public async Task<List<T>> GetList()
        {
            //Table  
            CloudTable table = await GetTableAsync(settings.TableName);
            //Query  
            TableQuery<T> query = new TableQuery<T>();
            List<T> results = new List<T>();
            TableContinuationToken continuationToken = null;
            do
            {
                TableQuerySegment<T> queryResults = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                continuationToken = queryResults.ContinuationToken;
                results.AddRange(queryResults.Results);
            } while (continuationToken != null);
            return results;
        }


        public async Task<T> GetItem(string rowKey)
        {
            //Table  
            CloudTable table = await GetTableAsync(settings.TableName);
            //Operation  
            TableOperation operation = TableOperation.Retrieve<T>("", rowKey);
            //Execute  
            TableResult result = await table.ExecuteAsync(operation);
            return (T)(dynamic)result.Result;
        }

        public async Task<T> GetItemtoDelete(string partitionKey,string rowKey)
        {
            //Table  
            CloudTable table = await GetTableAsync(settings.TableName);
            //Operation  
            TableOperation operation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            //Execute  
            TableResult result = await table.ExecuteAsync(operation);
            return (T)(dynamic)result.Result;
        }
        public async Task Insert(T item)
        {
            //Table  
            CloudTable table = await GetTableAsync(settings.TableName);
            //Operation  
            TableOperation operation = TableOperation.Insert(item);
            //Execute  
            await table.ExecuteAsync(operation);
        }
        public async Task Update(T item)
        {
            //Table  
            CloudTable table = await GetTableAsync(settings.TableName);
            //Operation  
            TableOperation operation = TableOperation.InsertOrReplace(item);
            //Execute  
            await table.ExecuteAsync(operation);
        }
        public async Task Delete(string partitionKey,string rowKey)
        {
            //Item  
            T item = await GetItemtoDelete(partitionKey, rowKey);
            //Table  
            CloudTable table = await GetTableAsync(settings.TableName);
            //Operation  
            TableOperation operation = TableOperation.Delete(item);
            //Execute  
            await table.ExecuteAsync(operation);
        }
        private async Task<CloudTable> GetTableAsync(string Tablename)
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount( new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
            ""secretkey1"", "Vs6lSzauuCOL3J81QMfwf9v+"secretkey"/VPAu9jUFg=="), true);

            string storageConnectionString = "DefaultEndpointsProtocol=https;"
                                            + "AccountName="secretkey1""
                                            + ";AccountKey=Vs6lSzauuCOL3J81QMfwf9v+"secretkey"/VPAu9jUFg=="
                                            + ";EndpointSuffix=core.windows.net";
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient cloudTableClient = account.CreateCloudTableClient();
            //Table  
            CloudTable table = cloudTableClient.GetTableReference(Tablename);
            await table.CreateIfNotExistsAsync();
            return table;
        }


    }
}
