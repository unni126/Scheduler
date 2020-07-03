using System.Collections.Generic;

namespace BEO.Scheduler.Models
{
    public class BookPassengersViewModel 
    {
        public int AppointmentId { get; set; }
        public List<PassengerViewModel> Passengers { get; set; }
    }
}
