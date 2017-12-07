using System;
using System.Linq;
using System.Web.Mvc;
using ShipIt.Models;
using ShipIt.ViewModels;

namespace ShipIt.Controllers
{
    public class BetsController : Controller
    {
        private ApplicationDbContext _context;

        public BetsController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: Bet
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult New()
        {
            var betStatus = _context.BetStatus.ToList();

            var viewModel = new BetFormViewModel
            {
                BetStatus = betStatus
            };

            return View("BetForm", viewModel);
        }

        [HttpPost]
        public ActionResult Save(Bet bet)
        {
            var betStatus = _context.BetStatus.ToList();

            var viewModel = new BetFormViewModel
            {
                BetStatus = betStatus
            };

            bet.StartDate = DateTime.Now;
            _context.Bets.Add(bet);

            _context.SaveChanges();

            return RedirectToAction("Index", "Bets");
        }
    }
}