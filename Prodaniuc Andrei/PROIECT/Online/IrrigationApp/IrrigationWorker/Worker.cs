using IrrigationWorker.Services;
using System;
using System.Collections.Generic;
using static IrrigationWorker.Services.DAL;

namespace IrrigationWorker
{
    public class Worker
    {
        private List<string> usersId;
        private List<AreaModel> areas;
        private List<SensorModel> sensors;

        private DAL dal; 

        public Worker()
        {
            dal = new Services.DAL();
        }

        public void Init()
        {
            usersId = dal.GetUsersId().Result;

        }

        public void Process()
        {
            foreach(var id in usersId)
            {
                areas = dal.GetAreas(id);
                foreach(var a in areas)
                {
                    WeatherComputation.CreateProjection(a.Id);
                    WeatherComputation.UpdateSensorValue(a.Id);
                }
            }
        }
    }
}