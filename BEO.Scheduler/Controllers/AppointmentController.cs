using BEO.Scheduler.Core.Helpers;
using BEO.Scheduler.Core.Models;
using BEO.Scheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scheduler.Core.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BEO.Scheduler.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly SchedulerContext _context;

        public AppointmentController(SchedulerContext context)
        {
            _context = context;
        }

        // GET: Appointment
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments.ToListAsync();
            var viewModel = from appointment in appointments
                            select new AppointmentViewModel
                            {
                                Id = appointment.Id,
                                AppointmentDate = appointment.AppointmentDate,
                                Capacity = appointment.Capacity,
                                Passengers = appointment.Passengers,
                                Status = appointment.Status
                            };
            return View(viewModel);
        }

        // GET: Appointment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.Include(p => p.Passengers)
                .FirstOrDefaultAsync(m => m.Id == id);
            var viewModel = new AppointmentViewModel
            {
                Id = appointment.Id,
                AppointmentDate = appointment.AppointmentDate,
                Capacity = appointment.Capacity,
                Passengers = appointment.Passengers,
                Status = appointment.Status
            };
            if (appointment == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        // GET: Appointment/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Appointment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentDate,Capacity,Status")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.Status = AppointmentStatus.NotConfirmed;
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(appointment);
        }

        // GET: Appointment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }
            var viewModel = new AppointmentViewModel
            {
                Id = appointment.Id,
                AppointmentDate = appointment.AppointmentDate,
                Capacity = appointment.Capacity,
                Passengers = appointment.Passengers,
                Status = appointment.Status
            };
            return View(viewModel);
        }

        // POST: Appointment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppointmentDate,Capacity,Status")] AppointmentViewModel appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var _appointment = await _context.Appointments.Include(p => p.Passengers).FirstOrDefaultAsync(m => m.Id == id);

                    if (!IsValidStatusChange(_appointment.Status, appointment.Status))
                    {
                        var errorModel = new ErrorViewModel
                        {
                            Message = "This status change is not permitted",
                            ActionName = "Edit",
                            ControllerName = "Appointment"
                        };
                        return View("Error", errorModel);
                    }

                    if (appointment.Status == AppointmentStatus.Denied)
                    {
                        await UpdatePassengerStatus(_appointment, PassengerStatus.Schedule, AppointmentStatus.Denied);
                        _appointment.Status = AppointmentStatus.Denied;
                    }
                    if (appointment.Status == AppointmentStatus.Confirmed)
                    {
                        await UpdatePassengerStatus(_appointment, PassengerStatus.SuccessfullFlight, AppointmentStatus.Confirmed);
                        _appointment.Status = AppointmentStatus.Confirmed;
                    }
                    if (appointment.Status == AppointmentStatus.NotConfirmed)
                    {
                        _appointment.Status = AppointmentStatus.NotConfirmed;
                    }//if's should be replace with switch


                    _appointment.AppointmentDate = appointment.AppointmentDate;
                    _appointment.Capacity = appointment.Capacity;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(appointment);
        }

        private static bool IsValidStatusChange(AppointmentStatus prev, AppointmentStatus curr)
        {
            if (prev == curr)
            {
                return true; // this is an edit on other values
            }
            if (prev == AppointmentStatus.NotConfirmed && (curr == AppointmentStatus.Confirmed || curr == AppointmentStatus.Denied))
            {
                return true; // this is an edit on status change or other values
            }
            else
            {
                return false; // cant change from confirmed to notconfirmed / denied and vice-versa
            }
        }


        private async Task UpdatePassengerStatus(Appointment appointment, PassengerStatus status, AppointmentStatus appointmentStatus)
        {
            foreach (var passenger in appointment.Passengers)
            {
                if (appointmentStatus == AppointmentStatus.Denied)
                {
                    passenger.AppointmentId = null;
                }
                passenger.Status = status;
            }
        }


        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }

        public async Task<IActionResult> BookPassengers(int id)
        {
            var passengers = await _context.Passengers.Where(p => p.AppointmentId != id && p.Status != PassengerStatus.SuccessfullFlight).ToListAsync();

            var model = from passenger in passengers
                        select new PassengerViewModel
                        {
                            Id = passenger.Id,
                            FirstName = passenger.FirstName,
                            LastName = passenger.LastName,
                            Status = passenger.Status,
                            Weight = passenger.Weight
                        };
            var viewModel = new BookPassengersViewModel
            {
                AppointmentId = id,
                Passengers = model.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookPassengers(BookPassengersViewModel booking)
        {
            var appointment = await _context.Appointments.Include(p => p.Passengers).FirstOrDefaultAsync(x => x.Id == booking.AppointmentId);
            if (appointment == null)
            {
                return NotFound();
            }
            if (booking.Passengers == null)
            {
                return RedirectToAction("Index"); //should handle the scenario, now redirecting to index page
            }

            var weightTobeAdded = booking.Passengers.Where(p => p.IsSelected).Sum(s => s.Weight);
            var currentWeight = appointment.Passengers == null ? 0 : appointment.Passengers.Sum(s => s.Weight);

            if (appointment.Capacity < weightTobeAdded + currentWeight)
            {
                var errorModel = new ErrorViewModel
                {
                    Message = "Cannot schedule the Appointment, capacity over flow.",
                    ActionName = "BookPassengers",
                    ControllerName = "Appointment"
                };
                return View("Error", errorModel);
            }

            if (appointment.Passengers == null)
            {
                appointment.Passengers = new List<Passenger>();
            }

            foreach (var passenger in booking.Passengers.Where(p => p.IsSelected))
            {
                appointment.Passengers.Add(new Passenger
                {
                    Id = passenger.Id,
                    FirstName = passenger.FirstName,
                    LastName = passenger.LastName,
                    Weight = passenger.Weight,
                    Status = appointment.Status == AppointmentStatus.Confirmed ? PassengerStatus.SuccessfullFlight : PassengerStatus.Active,
                    AppointmentId = booking.AppointmentId
                });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Schedule(int id)
        {
            ViewBag.passengerId = id;
            var passenger = await _context.Passengers.FirstOrDefaultAsync(p => p.Id == id);

            var appointments = await _context.Appointments.Where(a => a.Status != AppointmentStatus.Denied && a.Id != passenger.AppointmentId).ToListAsync();
            var viewModel = from appointment in appointments
                            select new AppointmentViewModel
                            {
                                Id = appointment.Id,
                                AppointmentDate = appointment.AppointmentDate,
                                Capacity = appointment.Capacity,
                                Status = appointment.Status
                            };
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Schedule(int passengerId, int appointmentId)
        {
            var passenger = await _context.Passengers.FindAsync(passengerId);
            var appointment = await _context.Appointments.Include(p=>p.Passengers).FirstOrDefaultAsync(a=>a.Id==appointmentId);
            var currentWeight = appointment.Passengers == null ? 0 : appointment.Passengers.Sum(s => s.Weight);
            var totalWeight = currentWeight + passenger.Weight;

            if (appointment.Capacity < totalWeight)
            {
                var errorModel = new ErrorViewModel
                {
                    Message = "Cannot schedule the Appointment, capacity over flow.",
                    ActionName = "Schedule",
                    ControllerName = "Appointment"
                };
                return View("Error", errorModel);
            }
            else
            {
                passenger.AppointmentId = appointmentId;
                passenger.Status = appointment.Status == AppointmentStatus.Confirmed ? PassengerStatus.SuccessfullFlight : PassengerStatus.Active;
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Passenger", new { id = passengerId });

            }
        }
    }
}
