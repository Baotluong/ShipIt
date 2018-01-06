using System;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using ShipIt.Models;
using ShipIt.ViewModels;

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

        // GET /api/mybets
        [HttpGet]
        [Route("api/mybets")]
        public IHttpActionResult GetMyBets(string query = null)
        {
            string currentUserId = User.Identity.GetUserId();

            ApplicationUser currentUser = _context.Users.First(u => u.Id == currentUserId);

            return Ok(currentUser.Bets.Select(b => {
                var User1InDb = b.ApplicationUsers.ElementAtOrDefault(0);
                var User2InDb = b.ApplicationUsers.ElementAtOrDefault(1);

                return new MyBetsViewModel()
                {
                    BetFee = b.BetFee,
                    BetPremise = b.BetPremise,
                    User1 = (User1InDb == null) ? "" : User1InDb.Email,
                    User1Condition = b.Conditions.FirstOrDefault(c => c.ApplicationUser == User1InDb).WinCondition,
                    User2 = (User2InDb == null) ? "" : User2InDb.Email,
                    User2Condition = b.Conditions.FirstOrDefault(c => c.ApplicationUser == User2InDb).WinCondition,
                    EndDate = b.EndTime,
                    StartDate = b.StartDate,
                    BetId = b.Id.ToString(),
                    BetStatus = Enum.GetName(typeof(BetStatus), b.BetStatus)
                };
            }));
        }

        // GET /api/bets/1
        public IHttpActionResult GetBet(string id)
        {
            var betInDb = _context.Bets.Where(b => b.Id.ToString() == id).SingleOrDefault();
            var User1InDb = betInDb.ApplicationUsers.ElementAtOrDefault(0);
            var User2InDb = betInDb.ApplicationUsers.ElementAtOrDefault(1);

            //TODO: Change this ViewModel
            var NewBetViewModel = new NewBetViewModel
            {
                BetFee = betInDb.BetFee,
                BetPremise = betInDb.BetPremise,
                User1 = (User1InDb == null) ? "" : User1InDb.Email,
                User1Condition = betInDb.Conditions.Where(c => c.ApplicationUser == User1InDb).SingleOrDefault().WinCondition,
                User2 = (User2InDb == null) ? "" : User2InDb.Email,
                User2Condition = betInDb.Conditions.Where(c => c.ApplicationUser == User2InDb).SingleOrDefault().WinCondition,
                EndTime = betInDb.EndTime
            };

            return Ok(NewBetViewModel);
        }

        //DO NOT NEED A POST API AT THE MOMENT
        //[HttpPost]
        //public IHttpActionResult CreateBet(NewBetViewModel newBetViewModel)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest();

        //    var newBet = new Bet();

        //    var newBetUserConditions = new List<KeyValuePair<string, string>>() {
        //        new KeyValuePair<string, string>(newBetViewModel.User1, newBetViewModel.User1Condition),
        //        new KeyValuePair<string, string>(newBetViewModel.User2, newBetViewModel.User2Condition)
        //    };

        //    List<ApplicationUser> UsersInDb = new List<ApplicationUser>();
        //    foreach (KeyValuePair<string, string> userConditions in newBetUserConditions)
        //    {
        //        var UserinDb = _context.Users
        //            .Where(u => u.Email == userConditions.Key).SingleOrDefault();
        //        UsersInDb.Add(UserinDb);
        //    }

        //    newBet.StartDate = DateTime.Now;
        //    newBet.BetStatusId = 1;
        //    newBet.EndTime = newBetViewModel.EndTime;
        //    newBet.BetFee = newBetViewModel.BetFee;
        //    newBet.ApplicationUsers = UsersInDb;
        //    string currentUserId = User.Identity.GetUserId();
        //    newBet.BetCreatorId = _context.Users.Where(u => u.Id == currentUserId).SingleOrDefault().Id;

        //    List<Condition> NewBetConditions = new List<Condition>();
        //    foreach (KeyValuePair<string, string> userConditions in newBetUserConditions)
        //    {
        //        var UserinDb = _context.Users
        //            .Where(u => u.Email == userConditions.Key).SingleOrDefault();
        //        var newCondition = new Condition()
        //        {
        //            WinCondition = userConditions.Value,
        //            Bet = newBet,
        //            ApplicationUser = UserinDb
        //        };
        //        NewBetConditions.Add(newCondition);
        //    }
        //    newBet.Conditions = NewBetConditions;

        //    _context.Bets.Add(newBet);
        //    _context.SaveChanges();

        //    return Created(new Uri(Request.RequestUri + "/" + newBet.Id), newBet);
        //}
    }
}
