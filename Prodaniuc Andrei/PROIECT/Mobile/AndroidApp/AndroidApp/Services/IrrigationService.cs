﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Net.Http.Headers;
using Android.Gms.Maps.Model;

namespace MyApp.Services
{
    public class IrrigationService
    {
        private const string baseUrl = "http://irrigationwebapidatc.azurewebsites.net/";
        //private const string baseUrl = "http://localhost:16346/";
        //private const string baseUrl = "http://10.0.2.2:16346/";
        private HttpClient client;
        public IrrigationService()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
        }

        public async Task<UserSetUpModel> GetLogin(string username, string password)
        {
            var pwd = EncryptPassword(password);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var credentials = new UserLogin() { Email = username, Password = pwd };
            var content = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");
            try
            {
                var result = await client.PostAsync("api/Account/CheckPwd", content);

                if (result.IsSuccessStatusCode)
                {
                    var str = await result.Content.ReadAsStringAsync();
                    var resp = JsonConvert.DeserializeObject<UserSetUpModel>(str);
                    return resp;
                }
                return null;
            }
            catch (Exception x)
            {
                return null;
            }
        }




        public string EncryptPassword(string password)
        {
            SHA1 hash = SHA1.Create();
            var data = System.Text.Encoding.UTF8.GetBytes(password);
            var result = hash.ComputeHash(data);
            return System.Text.Encoding.UTF8.GetString(result);
        }



        public async Task<bool> PostSensorsForArea(List<LatLng> sensors, string areaId)
        {
            List<SensorModel> sensorsList = new List<SensorModel>();
            foreach (var sensor in sensors)
            {
                sensorsList.Add(new SensorModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    AreaId = areaId,
                    Lat=sensor.Latitude,
                    Lng=sensor.Longitude
                });
            }
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var content = new StringContent(JsonConvert.SerializeObject(sensorsList), Encoding.UTF8, "application/json");
            try
            {
                var result = await client.PostAsync("api/Values/AddSensor", content);

                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
            catch (Exception x)
            {
                return false;
            }
        }
    }

    public class UserLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserSetUpModel
    {
        public string Id { get; set; }
        public bool IsSetUp { get; set; }
    }

    public class SensorModel
    {
        public string Id { get; set; }
        public string AreaId { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public bool IsActive { get; set; }
    }
}