using IrrigationWorker.Models;
using IrrigationWorker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IrrigationWorker.Services.DAL;

namespace IrrigationWorker
{
    public static class WeatherComputation
    {
        private static DAL _dal;
        public static void CreateProjection(string area)
        {
            _dal = new DAL();

            var result = _dal.GetSensorsForArea(area).Result;

            List<LatLng> data = new List<LatLng>();
            float average = 0;
            foreach (SensorModel sensor in result)
            {
                average += sensor.Value;
                if (sensor.Value<3)
                {
                    var val = new LatLng() { Latitude = sensor.Lat, Longitude = sensor.Lng };
                    data.Add(val);
                }
            }
            average = average / result.Count;
            _dal.AddData(data, average);
        }

        public static void UpdateSensorValue(string area)
        {
            _dal = new DAL();
            Random random = new Random();


            var result = _dal.GetSensorsForArea(area).Result;

            foreach(SensorModel sensor in result)
            {
                if(sensor.IsActive)
                _dal.UpdateSensorsValue(sensor.Id, random.Next(1,5));
            }
        }

    }
}
