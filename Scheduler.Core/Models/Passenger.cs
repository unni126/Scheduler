using BEO.Scheduler.Core.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BEO.Scheduler.Core.Models
{
    [Table("Passengers")]
    public class Passenger
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Weight { get; set; }
        public PassengerStatus Status { get; set; }
        public int? AppointmentId { get; set; }
        public virtual Appointment Appointment { get; set; }
    }
}
