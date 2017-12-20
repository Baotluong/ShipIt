using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ShipIt.Models;
using ShipIt.ViewModels;
using System.Web.Security;
using Microsoft.AspNet.Identity;

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
            string currentUserId = User.Identity.GetUserId();
            string currentUserEmail = _context.Users.Where(u => u.Id == currentUserId).SingleOrDefault().Email;

            var viewModel = new BetFormViewModel
            {
                BetStatus = betStatus,
                CurrentUserEmail = currentUserEmail
            };

            return View("BetForm", viewModel);
        }

        [HttpPost]
        public ActionResult Save(NewBetViewModel bet)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new NewBetViewModel
                {
                    BetFee = bet.BetFee,
                    EndTime = bet.EndTime,
                    User1 = bet.User1,
                    User1Condition = bet.User1Condition,
                    User2 = bet.User2,
                    User2Condition = bet.User2Condition
                };
                return RedirectToAction("New", viewModel);
            }

            var newBet = new Bet();

            var newBetUserConditions= new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>(bet.User1, bet.User1Condition),
                new KeyValuePair<string, string>(bet.User2, bet.User2Condition)
            };

            List<ApplicationUser> UsersInDb = new List<ApplicationUser>();
            foreach (KeyValuePair<string, string> userConditions in newBetUserConditions)
            {
                var UserinDb = _context.Users
                    .Where(u => u.Email == userConditions.Key).SingleOrDefault();
                UsersInDb.Add(UserinDb);
            }

            newBet.StartDate = DateTime.Now;
            newBet.EndTime = bet.EndTime;
            newBet.BetFee = bet.BetFee;
            newBet.ApplicationUsers = UsersInDb;
            string currentUserId = User.Identity.GetUserId();
            newBet.BetCreatorId = _context.Users.Where(u => u.Id == currentUserId).SingleOrDefault().Id;

            List<Condition> NewBetConditions = new List<Condition>();
            foreach (KeyValuePair<string, string> userConditions in newBetUserConditions)
            {
                var UserinDb = _context.Users
                    .Where(u => u.Email == userConditions.Key).SingleOrDefault();
                var newCondition = new Condition()
                {
                    WinCondition = userConditions.Value,
                    Bet = newBet,
                    ApplicationUser = UserinDb
                };
                NewBetConditions.Add(newCondition);
            }
            newBet.Conditions = NewBetConditions;

            _context.Bets.Add(newBet);
            _context.SaveChanges();

            return RedirectToAction("Index", "Bets");
        }
    }
}