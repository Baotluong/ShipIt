using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using ShipIt.Models;
using ShipIt.ViewModels;
using System.Data.Entity;

namespace ShipIt.Controllers.API
{
    //finish API stuff
    public class BetsController : ApiController
    {
        private ApplicationDbContext _context;

        public BetsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET /api/bets
        public IHttpActionResult GetBets(string query = null)
        {
            IEnumerable<Bet> betsQuery = _context.Bets
                .Include(b => b.ApplicationUsers)
                .Include(b => b.Conditions)
                .ToList();

            string currentUserId = User.Identity.GetUserId();
            string currentUserEmail = _context.Users.Where(u => u.Id == currentUserId).SingleOrDefault().Email;

            IEnumerable<ApplicationUser> usersBets = _context.Users.Where(u => u.Id == currentUserId).ToList();

            //if (!String.IsNullOrWhiteSpace(query))
            //betsQuery = betsQuery.Where(b => b.ApplicationUsers.ToDictionary(id)

            return Ok(betsQuery);
        }

        // GET /api/bets/1
        public IHttpActionResult GetBet(string id)
        {
            var betInDb = _context.Bets.Where(b => b.Id.ToString() == id).SingleOrDefault();
            var User1InDb = betInDb.ApplicationUsers.ElementAt(0);
            var User2InDb = betInDb.ApplicationUsers.ElementAt(1);

            var NewBetViewModel = new NewBetViewModel
            {
                BetFee = betInDb.BetFee,
                BetPremise = betInDb.BetPremise,
                User1 = User1InDb.Email,
                User1Condition = betInDb.Conditions.Where(c => c.ApplicationUser == User1InDb).SingleOrDefault().WinCondition,
                User2 = User2InDb.Email,
                User2Condition = betInDb.Conditions.Where(c => c.ApplicationUser == User2InDb).SingleOrDefault().WinCondition,
                EndTime = betInDb.EndTime
            };

            var BetFormViewModel = new BetFormViewModel
            {
                NewBetViewModel = NewBetViewModel,
            };

            return Ok(BetFormViewModel);
        }

        [HttpPost]
        public IHttpActionResult CreateBet(NewBetViewModel newBetViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var newBet = new Bet();

            var newBetUserConditions = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>(newBetViewModel.User1, newBetViewModel.User1Condition),
                new KeyValuePair<string, string>(newBetViewModel.User2, newBetViewModel.User2Condition)
            };

            List<ApplicationUser> UsersInDb = new List<ApplicationUser>();
            foreach (KeyValuePair<string, string> userConditions in newBetUserConditions)
            {
                var UserinDb = _context.Users
                    .Where(u => u.Email == userConditions.Key).SingleOrDefault();
                UsersInDb.Add(UserinDb);
            }

            newBet.StartDate = DateTime.Now;
            newBet.EndTime = newBetViewModel.EndTime;
            newBet.BetFee = newBetViewModel.BetFee;
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

            return Created(new Uri(Request.RequestUri + "/" + newBet.Id), newBet);
        }
    }
}
