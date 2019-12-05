//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using MBCEventDonation.Repository.EventDonation;
//using Microsoft.AspNetCore.Mvc;

//namespace MBCEventDonation.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class EventDonationController : Controller
//    {
//        private readonly IEventDonationService _service;
//        public EventDonationController(IEventDonationService service)
//        {
//            this._service = service;
//        }
//        [HttpGet]
//        public async Task<List<EventDonationModel>> Get()
//        {
//            var model = await _service.GetEventDonations();
//            return model.ToList();
//        }
//        [HttpGet("{id}")]
//        public async Task<EventDonationModel> Get(string id)
//        {
//            var model = await _service.GetEventDonation(id);
//            return model;
//        }
//        [HttpPost]
//        public async Task<bool> Create([FromBody] EventDonationModel eventDonation)
//        {
//            if (eventDonation == null) return false;
//            eventDonation.Id = Guid.NewGuid().ToString();
//            eventDonation.PartitionKey = "";
//            eventDonation.RowKey = Convert.ToString(eventDonation.Id);
//            if (!ModelState.IsValid) return false;
//            await _service.AddEventDonation(eventDonation);
//            return true;
//        }
//        [HttpPut("{id}")]
//        public async Task<bool> Update(string id, [FromBody] EventDonationModel eventDonation)
//        {
//            eventDonation.Id = id;
//            eventDonation.PartitionKey = "";
//            eventDonation.RowKey = eventDonation.Id;
//            if (!ModelState.IsValid) return false;
//            await _service.UpdateEventDonation(eventDonation);
//            return true;
//        }
//        [HttpDelete("{id}")]
//        public async Task Delete(string partitionkey, string RowKey)
//        {
//            await _service.DeleteEventDonation(partitionkey,RowKey);
//        }
//    }
//}