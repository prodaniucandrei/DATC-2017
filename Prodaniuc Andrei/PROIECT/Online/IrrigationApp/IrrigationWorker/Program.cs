using IrrigationWorker.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            DoWork();
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
