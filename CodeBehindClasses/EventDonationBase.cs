using Blazor.Extensions.Storage;
using MBCEventDonation.Repository.EventDonation;
using MBCEventDonation.Repository.Toast;
using Microsoft.AspNetCore.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MBCEventDonation.CodeBehindClasses
{
    public class EventDonationBase : ComponentBase, IDisposable

    {
        //public static IAzureTableStorage<EventDonationModel> _repository;
        //public EventDonationClass(IAzureTableStorage<EventDonationModel> repository)
        //{
        //    _repository = repository;
        //}

        //EventDonationService donationService = new EventDonationService(_repository);

        public List<EventDonationModel> eventDonations = new List<EventDonationModel>();
        public EventDonationModel objeventDonationModel = new EventDonationModel();

        [Parameter]
        public int LastEventDonationRunningTotal { get; set; }
        [Parameter]
        public int LastEventDonationPledgesRunningTotal { get; set; }
        [Parameter]
        public int LastEventDonationDueRunningTotal { get; set; }

        public bool ShowPopup = false;
        public bool isExportDisabled = false;
        public bool loadFailed = false;

        [Microsoft.AspNetCore.Components.Inject] public ToastService toastService { get; set; }
        [Microsoft.AspNetCore.Components.Inject] public EventDonationService DonationService { get; set; }
        [Microsoft.AspNetCore.Components.Inject] public NavigationManager NavigationManager { get; set; }
        [Microsoft.AspNetCore.Components.Inject] public ILogger<EventDonationBase> logger { get; set; }
        [Microsoft.AspNetCore.Components.Inject] public ProtectedSessionStorage ProtectedSessionStore { get; set; }

      
        //[Inject]
        //protected IRepository Repository { get; set; }
        //[Microsoft.AspNetCore.Components.Inject] public SessionStorage sessionStorage  { get; set; }
        //[Microsoft.AspNetCore.Components.Inject] public LocalStorage localStorage { get; set; }

        public async Task SavetoSessionAsync()
        {
            var keySessionStorage = "DonationServiceVariable";
            var keyLocalStorage = "DonationServiceVariable";

            await ProtectedSessionStore.SetAsync("var1", keySessionStorage);
            await ProtectedSessionStore.SetAsync("var2", keyLocalStorage);
        }

        public async Task DeleteEventDonation(string RowKey, string PartitionKey)
        {
            // Close the Popup
            ShowPopup = false;

            try
            {
                // Delete the EventDonation
                var result = await DonationService.DeleteEventDonationAsync(objeventDonationModel.PartitionKey, objeventDonationModel.RowKey);

                // Get the EventDonations for the current user
                eventDonations = await DonationService.GetEventDonationsAsync();
                CalculateMoneyOnHand();
            }
            catch (Exception)
            {

                throw;
            }
            toastService.ShowToast("Deleted record successfully", ToastLevel.Success);
            ShowDeletePopup = false;
        }
        public async Task DeleteEventDonationFromContextMenu(string RowKey, string PartitionKey)
        {
            // Close the Popup
            ShowPopup = false;
            try
            {
                // Delete the EventDonation
                var result = await DonationService.DeleteEventDonationAsync(RowKey, PartitionKey);

                // Get the EventDonations for the current user
                eventDonations = await DonationService.GetEventDonationsAsync();
                CalculateMoneyOnHand();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            toastService.ShowToast("Deleted record successfully", ToastLevel.Success);
            ShowDeletePopup = false;
        }
        public bool ShowDeletePopup;
        public void EditEventDonation(EventDonationModel eventDonationModel)
        {
            // Set the selected EventDonation
            // as the current EventDonation
            objeventDonationModel = eventDonationModel;

            // Open the Popup
            ShowPopup = true;
        }
        public void ClosePopup()
        {
            // Close the Popup
            ShowPopup = false;
        }
        public void CloseDeletePopup()
        {
            // Close the Popup
            ShowDeletePopup = false;
        }
        public void AddNewEventDonation()
        {
            // Make new EventDonation
            objeventDonationModel = new EventDonationModel();
            // Set Id to 0 so we know it is a new record
            //objeventDonationModel.Id = string.Empty;
            // Open the Popup
            ShowPopup = true;
        }
        public async Task SaveEventDonation()
        {
            // Close the Popup
            ShowPopup = false;
            try
            {
                // A new EventDonation will have the Id set to 0
                if (objeventDonationModel.RowKey == null)
                {
                    // Create new EventDonation
                    EventDonationModel objeventDonation = new EventDonationModel();
                    objeventDonation.PartitionKey = DateTime.UtcNow.Ticks.ToString();
                    objeventDonation.RowKey = DateTime.UtcNow.Ticks.ToString();
                    //objeventDonation.EventDate = System.DateTime.Now;
                    //objeventDonation.EventName = objeventDonationModel.EventName;
                    objeventDonation.Name = objeventDonationModel.Name;
                    objeventDonation.Money = objeventDonationModel.Money;
                    objeventDonation.PledgeMoney = objeventDonationModel.PledgeMoney;
                    // Save the result
                    var result = await DonationService.AddEventDonationAsync(objeventDonation);
                }
                else
                {
                    // This is an update
                    var result = await DonationService.UpdateEventDonationAsync(objeventDonationModel);
                }
                // Get the EventDonations for the current user
                eventDonations = await DonationService.GetEventDonationsAsync();
                CalculateMoneyOnHand();
                await SavetoSessionAsync();

                if (objeventDonationModel.RowKey == null)
                {
                    toastService.ShowToast("Added record successfully", ToastLevel.Success);
                }
                else
                {
                    toastService.ShowToast("Updated record successfully", ToastLevel.Success);
                }
            }
            catch (Exception)
            {

                throw;
            }
           
        }


        public void Navigate()
        {
            NavigationManager.NavigateTo("EventExpenses/" + LastEventDonationRunningTotal);
            CalculateMoneyOnHand();
        }
        protected void CalculateMoneyOnHand()
        {
            var total = 0;
            var pludgestotal = 0;
            var totaldue = 0;
            // eventDonations = await DonationService.GetEventDonationsAsync();
            foreach (var item in eventDonations)
            {
                total = item.Money + total;
                LastEventDonationRunningTotal = total;

                pludgestotal = item.PledgeMoney + pludgestotal;
                LastEventDonationPledgesRunningTotal = pludgestotal;

                totaldue = item.PledgeMoney - item.Money;
                LastEventDonationDueRunningTotal = totaldue;

            }

            this.StateHasChanged();
        }
        public async Task DownloadSpreadSheet()
        {
            await ExportButtonDisabledAsync();
        }
        public async Task ExportButtonDisabledAsync()
        {
            isExportDisabled = true;


            int timeout = 5000;
            var task = Task.Run(() => export());
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
                // task completed within timeout
                isExportDisabled = false;
                this.StateHasChanged();
            }
            else
            {
                // timeout logic
                isExportDisabled = false;
            }
            toastService.ShowToast("Exported to excel successfully", ToastLevel.Success);
        }
        public async Task export()
        {
            try
            {
                Task t = Task.Run(() =>
                {
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Donations");
                        var tableBody = worksheet.Cells["B2:B2"].LoadFromCollection(from D in eventDonations
                                                                                    select new { D.Name, D.Money, D.RunningTotal, D.PledgeMoney, D.PludgesRunningTotal, D.TotalDue }, true);
                        package.SaveAs(new FileInfo(@"C:\PracticeProjects\MBCEventDonation\EventDonations.xlsx"));
                        isExportDisabled = false;
                    }
                });
                TimeSpan ts = TimeSpan.FromMilliseconds(5000);
                if (!t.Wait(ts))
                {
                }
            }
            catch (Exception)
            {

                throw;
            }
          
        }
        public string V = string.Empty;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                V = "Salim bh oupu ";
                eventDonations = await DonationService.GetEventDonationsAsync();
                if (eventDonations.Count > 0)
                {
                    var LastEventDonationCount = eventDonations[eventDonations.Count - 1];
                    LastEventDonationRunningTotal = LastEventDonationCount.RunningTotal;
                    LastEventDonationPledgesRunningTotal = LastEventDonationCount.PludgesRunningTotal;
                    LastEventDonationDueRunningTotal = LastEventDonationPledgesRunningTotal - LastEventDonationRunningTotal;
                    //logging to consoel and output window
                    logger.LogInformation("logging from EventDonations page");

                    //// load from session
                    //var fromSession = await ProtectedSessionStore.GetAsync<string>("var1");
                    //var fromLocal = await ProtectedSessionStore.GetAsync<string>("var2");
                    await LoadStateAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + ex.StackTrace + "There was an error at { Time} ", DateTime.UtcNow);
            }

           
        }

        public async Task LoadStateAsync()
        {
            // load from session
            var fromSession = await ProtectedSessionStore.GetAsync<string>("var1");
            var fromLocal = await ProtectedSessionStore.GetAsync<string>("var2");
        }


        protected override async Task OnParametersSetAsync()
        {
            try
            {
                loadFailed = false;
               // details = await ProductRepository.GetProductByIdAsync(ProductId);
            }
            catch (Exception ex)
            {
                loadFailed = true;
               // Logger.LogWarning(ex, "Failed to load product {ProductId}", ProductId);
            }
        }

        public async void Dispose()
        {
            await SavetoSessionAsync();
            Console.WriteLine("Disposing AddressBase.");
        }


       

    }
}
