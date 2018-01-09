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
            WeatherComputation.CreateProjection("28df3166-8a6d-41ba-afa7-4161d7318266");
            WeatherComputation.UpdateSensorValue("28df3166-8a6d-41ba-afa7-4161d7318266");

            var worker = new Worker();
            //worker.Init();
            worker.Process();
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
