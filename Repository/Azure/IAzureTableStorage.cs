using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MBCEventDonation.Repository.Azure
{
    public interface IAzureTableStorage<T> where T : TableEntity, new()
    {
        Task Delete(string partitionKey,string rowKey);
        Task<T> GetItem(string rowKey);
        Task<List<T>> GetList();
        Task Insert(T item);
        Task Update(T item);

        Task<T> GetItemtoDelete(string partitionKey,string rowKey);
    }
}
