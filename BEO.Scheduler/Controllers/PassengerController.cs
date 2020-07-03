using BEO.Scheduler.Core.Helpers;
using BEO.Scheduler.Core.Models;
using BEO.Scheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scheduler.Core.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BEO.Scheduler.Controllers
{
    public class PassengerController : Controller
    {
        private readonly SchedulerContext _context;

        public PassengerController(SchedulerContext context)
        {
            _context = context;
        }

        // GET: Passenger
        public async Task<IActionResult> Index()
        {
            var passengers = await _context.Passengers.Include(p => p.Appointment).ToListAsync();
            var viewModel = from passenger in passengers
                            select new PassengerViewModel
                            {
                                Id = passenger.Id,
                                FirstName = passenger.FirstName,
                                LastName = passenger.LastName,
                                Status = passenger.Status,
                                Weight = passenger.Weight                                
                            };
            return View(viewModel);
        }

        // GET: Passenger/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var passenger = await _context.Passengers.Include(p => p.Appointment)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (passenger == null)
            {
                return NotFound();
            }
            var viewModel = new PassengerViewModel
                            {
                                Id = passenger.Id,
                                FirstName = passenger.FirstName,
                                LastName = passenger.LastName,
                                Status = passenger.Status,
                                Weight = passenger.Weight,
                                AppointmentId = passenger.AppointmentId,
                                Appointment = passenger.Appointment
                            };
            return View(viewModel);
        }

        // GET: Passenger/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Passenger/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Weight")] Passenger passenger)
        {
            if (ModelState.IsValid)
            {
                passenger.Status = PassengerStatus.Schedule;
                _context.Add(passenger);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(passenger);
        }

        // GET: Passenger/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var passenger = await _context.Passengers.FindAsync(id);
            if (passenger == null)
            {
                return NotFound();
            }
            var viewModel = new PassengerViewModel
            {
                Id = passenger.Id,
                FirstName = passenger.FirstName,
                LastName = passenger.LastName,
                Status = passenger.Status,
                Weight = passenger.Weight
            };
            return View(viewModel);
        }

        // POST: Passenger/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Weight,Status")] PassengerViewModel passenger)
        {
            if (id != passenger.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var _passenger = await _context.Passengers.FirstOrDefaultAsync(p => p.Id == id);
                    _passenger.FirstName = passenger.FirstName;
                    passenger.LastName = passenger.LastName;
                    _passenger.Weight = passenger.Weight;
                    _context.Update(_passenger);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PassengerExists(passenger.Id))
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
            return View(passenger);
        }
        

        private bool PassengerExists(int id)
        {
            return _context.Passengers.Any(e => e.Id == id);
        }
    }
}
