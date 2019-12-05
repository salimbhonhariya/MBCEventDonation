
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Table;

namespace MBCEventDonation.Repository.EventDonation
{
    public interface IEventDonationService
    {
        Task<bool> DeleteEventDonationAsync(string partitionKey, string rowKey);
        Task<bool> UpdateEventDonationAsync(EventDonationModel item);
        Task<EventDonationModel> AddEventDonationAsync(EventDonationModel item);
        Task<List<EventDonationModel>> GetEventDonationsAsync();
    }

    public class EventDonationModel : TableEntity
    {
        public EventDonationModel(string partitionKey, string rowKey,string Name, int Money, int plegdeMoney) : base(partitionKey, rowKey)
        {
            this.RowKey = rowKey;// DateTime.UtcNow.Ticks.ToString();
            this.PartitionKey = partitionKey;
            this.Name = Name;
            this.Money = Money;
            this.PledgeMoney = plegdeMoney;
        }

        public EventDonationModel()
        {
        }
        [Required(AllowEmptyStrings =false, ErrorMessage ="Please enter name")]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter amount")]
        [RegularExpression(@"^\$?\d+(\.(\d{2}))?$")]
        [MaxLength(10)]
        public int Money { get; set; }
        [RegularExpression(@"^\$?\d+(\.(\d{2}))?$")]
        [MaxLength(10)]
        public int PledgeMoney { get; set; }
        [DisplayName("Running Total")]
        public int RunningTotal { get; set; }
        public int PludgesRunningTotal { get;  set; }
        public 
            int TotalDue { get;  set; }
        public string email { get; set; }



    }
}
