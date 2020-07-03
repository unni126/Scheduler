using BEO.Scheduler.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BEO.Scheduler.Core.Models
{
    [Table("Appointments")]
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }        
        public int Capacity { get; set; }
        public AppointmentStatus Status { get; set; }
        public ICollection<Passenger> Passengers { get; set; }
    }
}
