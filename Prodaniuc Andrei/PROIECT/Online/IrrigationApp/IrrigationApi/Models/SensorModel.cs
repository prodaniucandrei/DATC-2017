using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IrrigationApi.Models
{
    public class SensorModel
    {
        public string Id { get; set; }
        public string AreaId { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public bool IsActive { get; set; }
        public int Value { get; set; }
    }
}