using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartyInvitesApp.Data;
using PartyInvitesApp.Models;

namespace PartyInvitesApp.Controllers
{
    public class PartyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PartyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Party
        public async Task<IActionResult> Index()
        {
            return View(await _context.Parties.ToListAsync());
        }

        // GET: Party/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Party/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Party party)
        {
            Console.WriteLine($"Received Party: {party.Description}, {party.EventDate}, {party.Location}");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(party);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("✅ Party saved successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ Error saving party: " + ex.Message);
                }

                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine("❌ ModelState is invalid. Errors:");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(party);
        }




        // GET: Party/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var party = await _context.Parties.FindAsync(id);
            if (party == null)
            {
                return NotFound();
            }
            return View(party);
        }

        // POST: Party/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,EventDate,Location")] Party party)
        {
            if (id != party.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(party);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PartyExists(party.Id))
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
            return View(party);
        }

        // GET: Party/Manage/5
        public async Task<IActionResult> Manage(int id)
        {
            var party = await _context.Parties
                .Include(p => p.Invitations) // Include related invitations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (party == null)
            {
                return NotFound();
            }
            return View(party);
        }



        private bool PartyExists(int id)
        {
            return _context.Parties.Any(e => e.Id == id);
        }
    }
}
