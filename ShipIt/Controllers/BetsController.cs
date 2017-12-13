using System;
using System.Collections.Generic;
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
            var betStatus = _context.BetStatuses.ToList();

            var viewModel = new BetFormViewModel
            {
                BetStatus = betStatus
            };

            return View("BetForm", viewModel);
        }

        [HttpPost]
        public ActionResult Save(NewBetViewModel bet)
        {
            var NewBet = new Bet();

            List<string> NewBetUsers = new List<string>(bet.Users.ToString().Split(',').ToList<string>());
            List<ApplicationUser> UsersInDb = new List<ApplicationUser>();
            foreach (string user in NewBetUsers)
            {
                var UserinDb = _context.Users
                    .Where(u => u.Email == user).SingleOrDefault();
                UsersInDb.Add(UserinDb);
            }
            //TODO: add the rest of the items and do a split on the user ids by comma. then search the users for the ids

            NewBet.StartDate = DateTime.Now;
            NewBet.EndTime = bet.EndTime;
            NewBet.BetFee = bet.BetFee;
            NewBet.ApplicationUsers = UsersInDb;

            _context.Bets.Add(NewBet);

            _context.SaveChanges();

            return RedirectToAction("Index", "Bets");
        }
    }
}