using BEO.Scheduler.Core.Models;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;

namespace Scheduler.Core.Data
{
    public class SchedulerContext : DbContext
    {
        public SchedulerContext([NotNullAttribute] DbContextOptions options) : base(options)
        {

        }

        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>()
                 .HasMany(p => p.Passengers)
                 .WithOne(a => a.Appointment);

            modelBuilder.Entity<Passenger>()
                 .HasOne(a => a.Appointment)
                 .WithMany(p=> p.Passengers);
        }
    }
}