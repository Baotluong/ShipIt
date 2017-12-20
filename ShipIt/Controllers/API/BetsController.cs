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
            var betsQuery = _context.Bets;
            string currentUserId = User.Identity.GetUserId();
            string currentUserEmail = _context.Users.Where(u => u.Id == currentUserId).SingleOrDefault().Email;

            //if (!String.IsNullOrWhiteSpace(query))
            //    betsQuery = betsQuery.Where(b => b.ApplicationUsers.Contains(currentUserId));

            return Ok(betsQuery);
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
