
using MBCEventDonation.Repository.Azure;
using MBCEventExpense.Repository.EventExpenses;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MBCEventExpense.Repository.EventExpenses
{
    public class EventExpenseService : IEventExpenseService
    {
        private readonly IAzureTableStorage<EventExpenseModel> _repository;

        public EventExpenseService(IAzureTableStorage<EventExpenseModel> repository)
        {
            this._repository = repository;
        }
        public async Task<EventExpenseModel> AddEventExpenseAsync(EventExpenseModel item)
        {
            EventExpenseModel eventExpenseModelEntity = new EventExpenseModel(item.PartitionKey, item.RowKey, item.Name, item.Money);
            await this._repository.Insert(eventExpenseModelEntity);
            return await Task.FromResult(item);
        }
        public async Task<List<EventExpenseModel>> GetEventExpensesAsync()
        {
            var acc = new CloudStorageAccount(new StorageCredentials("blobtransferutility", "Vs6lSzauuCOL3J81QMfwf9v+6ezGkwUUXRg5PoarJUCCEPsvP3wZHSHK0hrAp9FNhgH7KTPqJgeI/VPAu9jUFg=="), true);
            var tableClient = acc.CreateCloudTableClient();
            var table = tableClient.GetTableReference("EventExpense");
            await table.CreateIfNotExistsAsync();
            List<EventExpenseModel> eventExpenseModels = new List<EventExpenseModel>();
            TableContinuationToken tableContinuationToken = null;
            TableQuery<EventExpenseModel> query = new TableQuery<EventExpenseModel>();
            int previousEntityAmount = 0;
            foreach (EventExpenseModel singleentity in await table.ExecuteQuerySegmentedAsync(query, tableContinuationToken))
            {
                previousEntityAmount = previousEntityAmount + singleentity.Money;
                singleentity.RunningTotal = previousEntityAmount;
                eventExpenseModels.Add(singleentity);
            }
            return await Task.FromResult(eventExpenseModels);
            // return await this._repository.GetList();
        }
        public async Task<bool> UpdateEventExpenseAsync(EventExpenseModel item)
        {
            EventExpenseModel eventExpenseModelEntity = new EventExpenseModel(item.PartitionKey, item.RowKey, item.Name, item.Money);
            await this._repository.Update(eventExpenseModelEntity);
            return await Task.FromResult(true);
        }
        public async Task<bool> DeleteEventExpenseAsync(string partitionKey, string rowKey)
        {
            await this._repository.Delete(partitionKey, rowKey);
            return await Task.FromResult(true);
        }

    }
}


