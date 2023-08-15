using Homies.Data;
using Homies.Data.Models;
using Homies.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Xml.Linq;

namespace Homies.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        private readonly HomiesDbContext _ctx;
        public EventController(HomiesDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new EventFormViewModel();
            model.Types = GetTypes();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(EventFormViewModel model)
        {
            if (!GetTypes().Any(e => e.Id == model.TypeId))
            {
                ModelState.AddModelError(nameof(model.TypeId), "Type does not exist!");
            }

            if (!ModelState.IsValid)
            {
                model.Types = GetTypes();
                return View(model);
            }

            var userId = GetUserId();

            var entity = new Event()
            {
                Name = model.Name,
                Description = model.Description,
                TypeId = model.TypeId,
                Start = model.Start,
                CreatedOn = DateTime.UtcNow,
                End = model.End,
                OrganiserId = userId
            };

            await _ctx.Events.AddAsync(entity);
            await _ctx.SaveChangesAsync();

            return RedirectToAction(nameof(All));

        }

        public async Task<IActionResult> All()
        {
            var eventsToDisplay = await _ctx.Events
                .Select(e => new EventViewShortModel()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Start = e.Start.ToString("dd/MM/yyyy H:mm"),
                    Type = e.Type.Name,
                    Organiser = e.Organiser.UserName
                })
                .ToListAsync();

            return View(eventsToDisplay);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var eventDb = await _ctx.Events.FindAsync(id);

            if (eventDb == null)
            {
                return BadRequest();
            }

            if (GetUserId() != eventDb.OrganiserId)
            {
                return Unauthorized();
            }

            var eventTypes = GetTypes();

            var eventModel = new EventFormViewModel()
            {
                Id = eventDb.Id,
                Name = eventDb.Name,
                Description = eventDb.Description,
                TypeId = eventDb.TypeId,
                Start = eventDb.Start,
                End = eventDb.End,
                Types = eventTypes
            };

            return View(eventModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EventFormViewModel model)
        {
            var eventDb = await _ctx.Events.FindAsync(id);

            if (eventDb == null)
            {
                return BadRequest();
            }

            if (!GetTypes().Any(e => e.Id == model.TypeId))
            {
                ModelState.AddModelError(nameof(model.TypeId), "Type does not exist!");
            }

            if (GetUserId() != eventDb.OrganiserId)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                model.Types = GetTypes();
                return View(model);
            }

            eventDb.Name = model.Name;
            eventDb.Description = model.Description;
            eventDb.TypeId = model.TypeId;
            eventDb.Start = model.Start;
            eventDb.End = model.End;

            await _ctx.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            var eventToJoin = await _ctx
                .Events
                .FindAsync(id);

            if (eventToJoin == null)
            {
                return BadRequest();
            }

            var entity = new EventParticipant()
            {
                HelperId = GetUserId(),
                EventId = eventToJoin.Id
            };

            if (await _ctx.EventParticipants.ContainsAsync(entity))
            {
                return RedirectToAction(nameof(All));
            }

            await _ctx.EventParticipants.AddAsync(entity);
            await _ctx.SaveChangesAsync();

            return RedirectToAction(nameof(Joined));
        }

        [HttpPost]
        public async Task<IActionResult> Leave(int id)
        {
            var eventToLeave = await _ctx
                .Events
                .FindAsync(id);

            if (eventToLeave == null)
            {
                return BadRequest();
            }

            //if (eventToLeave.OrganiserId == GetUserId())
            //{
            //    return Unauthorized();
            //}

            var entity = await _ctx.EventParticipants
                .FirstOrDefaultAsync(e => e.EventId == eventToLeave.Id && e.HelperId == GetUserId());

            if (entity == null)
            {
                return BadRequest();
            }

            _ctx.EventParticipants.Remove(entity);
            await _ctx.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var currentEvent = await _ctx.Events
                .Where(e => e.Id == id)
                .Include(e => e.Organiser)
                .Include(e => e.Type)
                .FirstOrDefaultAsync();

            if (currentEvent == null)
            {
                return BadRequest();
            }

            var eventToDisplay = new EventDetailsViewModel()
            {
                Id = currentEvent.Id,
                Name = currentEvent.Name,
                Description = currentEvent.Description,
                Start = currentEvent.Start.ToString("dd/MM/yyyy H:mm"),
                End = currentEvent.End.ToString("dd/MM/yyyy H:mm"),
                Organiser = currentEvent.Organiser.UserName,
                CreatedOn = currentEvent.CreatedOn.ToString("dd/MM/yyyy H:mm"),
                Type = currentEvent.Type.Name
            };

            return View(eventToDisplay);
        }

        public async Task<IActionResult> Joined()
        {
            var userId = GetUserId();

            var joinedEventsIds = await _ctx
                .EventParticipants
                .Where(u => u.HelperId == userId)
                .Select(e => e.EventId)
                .ToListAsync();

            var userEvents = await _ctx.Events
                .Where(e => joinedEventsIds.Contains(e.Id))
                .Select(e => new EventViewShortModel()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Start = e.Start.ToString("dd/MM/yyyy H:mm"),
                    Type = e.Type.Name,
                    Organiser = e.Organiser.UserName
                })
                .ToListAsync();

            return View(userEvents);
        }

        private IEnumerable<EventTypeViewModel> GetTypes()
        {
            return _ctx.Types
                .Select(t => new EventTypeViewModel()
                {
                    Id = t.Id,
                    Name = t.Name
                });
        }

        private string GetUserId()
           => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
