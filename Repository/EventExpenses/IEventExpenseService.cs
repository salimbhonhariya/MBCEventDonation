using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MBCEventExpense.Repository.EventExpenses
{
    public interface IEventExpenseService
    {
        Task<bool> DeleteEventExpenseAsync(string partitionKey, string rowKey);
        Task<bool> UpdateEventExpenseAsync(EventExpenseModel item);
        Task<EventExpenseModel> AddEventExpenseAsync(EventExpenseModel item);
        Task<List<EventExpenseModel>> GetEventExpensesAsync();
    }

    public class EventExpenseModel : TableEntity
    {
        public EventExpenseModel(string partitionKey, string rowKey, string Name, int Money) : base(partitionKey, rowKey)
        {
            this.RowKey = rowKey;// DateTime.UtcNow.Ticks.ToString();
            this.PartitionKey = partitionKey;
            this.Name = Name;
            this.Money = Money;
        }

        public EventExpenseModel()
        {
        }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter name")]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter amount")]
        [RegularExpression(@"^\$?\d+(\.(\d{2}))?$")]
        [MaxLength(10)]
        public int Money { get; set; }
        [DisplayName("Running Total")]
        public int RunningTotal { get; set; }
    }


}


