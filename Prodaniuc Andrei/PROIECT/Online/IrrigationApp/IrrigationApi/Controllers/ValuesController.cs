using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Tracing;
using Microsoft.Azure.Mobile;
using IrrigationApi.Models;
using IrrigationApi.Service;
using System.Threading.Tasks;

namespace IrrigationApi.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/Values")]
    public class ValuesController : ApiController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        //{
        //    Service.DAL DAL = new Service.DAL();
        //    return DAL.GetWeatherInfo(); ;
        //}

        [HttpPost]
        [Route("AddSensor")]
        public async Task<IHttpActionResult> AddSensor(List<SensorModel> sensors)
        {
            DAL dal = new DAL();
            foreach (var item in sensors)
            {
                var result = dal.AddSensor(item);
            }
            return Ok("SensorsAdded");
        }

        [HttpPost]
        [Route("AddArea")]
        public async Task<IHttpActionResult> AddArea(AreaModel area)
        {
            DAL dal = new DAL();

            var result = dal.AddArea(area);
            return Ok("SensorsAdded");
        }

        [HttpGet]
        [Route("GetAreas")]
        public async Task<IEnumerable<AreaModel>> GetAreas(string userId)
        {
            DAL dal = new DAL();

            var result = dal.GetAreas(userId);
            return result;
        }


        [HttpGet]
        [Route("GetSensors")]
        public async Task<IEnumerable<SensorModel>> GetSensors(string areaId)
        {
            DAL dal = new DAL();

            var result = await dal.GetSensorsForArea(areaId);
            return result;
        }

        [HttpGet]
        [Route("GetData")]
        public async Task<Projection> GetData(string areaId)
        {
            DAL dal = new DAL();

            var result = await dal.GetDataForArea(areaId);

            return result;
        }
        // POST api/values
        public async void Post([FromBody]string value)
        {
            string notificationHubName = "";
            string notificationHubConnection = "NotificationHubConnectionString";

            // Create a new Notification Hub client.
            NotificationHubClient hub = NotificationHubClient
            .CreateClientFromConnectionString(notificationHubConnection, notificationHubName);

            // Sending the message so that all template registrations that contain "messageParam"
            // will receive the notifications. This includes APNS, GCM, WNS, and MPNS template registrations.
            Dictionary<string, string> templateParams = new Dictionary<string, string>();
            templateParams["messageParam"] = value + " was added to the list.";

            try
            {
                // Send the push notification and log the results.
                var result = await hub.SendTemplateNotificationAsync(templateParams);
            }
            catch (System.Exception ex)
            {
                // Write the failure result to the logs.
                Console.WriteLine(ex.Message);
            }
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
