using IrrigationWorker.Models;
using IrrigationWorker.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static IrrigationWorker.Services.DAL;

namespace IrrigationWorker
{
    class Program
    {
        private static GeoService _service;
        private static DAL _dal;
        static void Main(string[] args)
        {
            _service = new GeoService();
            _dal = new DAL();

            var result = _dal.GetSensorsForArea().Result;

            List<LatLng> data = new List<LatLng>();
            foreach(SensorModel sensor in result)
            {
                var val = new LatLng() { Latitude = sensor.Lat, Longitude = sensor.Lng};
                data.Add(val);
            }
            _dal.AddData(data);
            //DoWork();
        }

        private static void DoWork()
        {
            while (true)
            {
                var result = _service.GetWeatherAsync("45.127443", "21.281306").Result;
                _dal.SaveWeatherInfo(result);
                Thread.Sleep(TimeSpan.FromMinutes(30));
            }
        }
    }
}
