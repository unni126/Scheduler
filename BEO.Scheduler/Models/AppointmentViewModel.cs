using BEO.Scheduler.Core.Helpers;
using BEO.Scheduler.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BEO.Scheduler.Models
{
    public class AppointmentViewModel : Appointment
    {
        public AppointmentViewModel()
        {
            AppointmentStatus = Enum.GetNames(typeof(AppointmentStatus)).Select(name => new SelectListItem
            {
                Text = name,
                Value = name
            });
        }
        public int TotalWeight { get { return Passengers.Sum(s => s.Weight); } }
        public int TotalPassengers { get { return Passengers.Count; } }
        public IEnumerable<SelectListItem> AppointmentStatus { get; set; }
    }
}
