using BEO.Scheduler.Core.Helpers;
using BEO.Scheduler.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BEO.Scheduler.Models
{
    public class PassengerViewModel : Passenger
    {
        public PassengerViewModel()
        {
            PassengerStatus = Enum.GetNames(typeof(PassengerStatus)).Select(name => new SelectListItem
            {
                Text = name,
                Value = name
            });
        }
        public IEnumerable<SelectListItem> PassengerStatus { get; set; }
        public bool IsSelected { get; set; }

    }
}
