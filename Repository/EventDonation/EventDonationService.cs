using MBCEventDonation.Repository.Azure;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MBCEventDonation.Repository.EventDonation
{
    public class EventDonationService : IEventDonationService
    {
        private readonly IAzureTableStorage<EventDonationModel> _repository;
        public EventDonationService(IAzureTableStorage<EventDonationModel> repository)
        {
            this._repository = repository;
        }

        public EventDonationService()
        {
           
        }
        public async Task<EventDonationModel> AddEventDonationAsync(EventDonationModel item)
        {
            EventDonationModel eventDonationModelEntity = new EventDonationModel(item.PartitionKey, item.RowKey, item.Name, item.Money, item.PledgeMoney);
            await this._repository.Insert(eventDonationModelEntity);
            return await Task.FromResult(item);
        }
        public async Task<List<EventDonationModel>> GetEventDonationsAsync()
        {
            var acc = new CloudStorageAccount(new StorageCredentials(""secretkey1"", "Vs6lSzauuCOL3J81QMfwf9v+"secretkey"/VPAu9jUFg=="), true);
            var tableClient = acc.CreateCloudTableClient();
            var table = tableClient.GetTableReference("EventDonation");
            await table.CreateIfNotExistsAsync();
            List<EventDonationModel> eventDonationModels = new List<EventDonationModel>();
            TableContinuationToken tableContinuationToken = null;
            TableQuery<EventDonationModel> query = new TableQuery<EventDonationModel>();
            int previousEntityAmount = 0;
            int previousEntityPledgeAmount = 0;
            int previousEntityDue = 0;
            foreach (EventDonationModel singleentity in await table.ExecuteQuerySegmentedAsync(query, tableContinuationToken))
            {
                previousEntityAmount = previousEntityAmount + singleentity.Money;
                singleentity.RunningTotal = previousEntityAmount;


                previousEntityPledgeAmount = previousEntityPledgeAmount + singleentity.PledgeMoney;
                singleentity.PludgesRunningTotal = previousEntityPledgeAmount;

                previousEntityDue = (singleentity.PledgeMoney - singleentity.Money);
                singleentity.TotalDue = previousEntityDue;


                eventDonationModels.Add(singleentity);
            }
            return await Task.FromResult(eventDonationModels);
            // return await this._repository.GetList();
        }
        public async Task<bool> UpdateEventDonationAsync(EventDonationModel item)
        {
            EventDonationModel eventDonationModelEntity = new EventDonationModel(item.PartitionKey, item.RowKey, item.Name, item.Money, item.PledgeMoney);
            await this._repository.Update(eventDonationModelEntity);
            return await Task.FromResult(true);
        }
        public async Task<bool> DeleteEventDonationAsync(string partitionKey, string rowKey)
        {
            await this._repository.Delete(partitionKey, rowKey);
            return await Task.FromResult(true);
        }

    }
}
