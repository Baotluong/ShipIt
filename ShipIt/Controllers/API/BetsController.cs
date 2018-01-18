using System;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using ShipIt.Models;
using ShipIt.ViewModels;
using System.Collections.Generic;

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

        [Route("api/bets/{email?}")]
        public IHttpActionResult GetBetsIndex(string email = null)
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = _context.Users.Single(u => u.Id == currentUserId);

            ApplicationUser indexUser = (email == null) ? currentUser : _context.Users.Single(u => u.Email == email);

            var indexQuery = indexUser.Bets.Select(b => new BetsIndexViewModel()).ToList();

            var betsQueryIsBetCreator = _context.Bets.Where(b => b.BetCreatorId == indexUser.Id).ToList();
            var betsQueryIsABettor = indexUser.Bets.ToList();
            var betsQuery = betsQueryIsABettor.Union(betsQueryIsBetCreator).ToList();

            var betsIndexViewModelList = new List<BetsIndexViewModel>();

            foreach (Bet bet in betsQuery)
            {
                var User1InDb = bet.Conditions.ElementAt(0);
                var User2InDb = bet.Conditions.ElementAt(1);

                var betsIndexViewModel = new BetsIndexViewModel()
                {
                    BetWager = bet.BetWager,
                    BetPremise = bet.BetPremise,
                    User1 = User1InDb.UserEmail,
                    User1Condition = User1InDb.WinCondition,
                    User2 = User2InDb.UserEmail,
                    User2Condition = User2InDb.WinCondition,
                    EndDate = bet.EndTime,
                    StartDate = bet.StartDate,
                    BetId = bet.Id.ToString(),
                    BetStatus = Enum.GetName(typeof(BetStatus), bet.BetStatus),
                    BetWinner = (bet.ProposedBetWinner == null) ? "No Winner Yet" : (bet.BetWinner == null) ? bet.ProposedBetWinner+"*": bet.BetWinner
                };
                betsIndexViewModelList.Add(betsIndexViewModel);
            }

            return Ok(betsIndexViewModelList);
        }


        //NOT USING THIS METHOD RIGHT NOW
        //// GET /api/bets/1
        //public IHttpActionResult GetBet(string id)
        //{
        //    var betInDb = _context.Bets.Where(b => b.Id.ToString() == id).SingleOrDefault();
        //    var User1InDb = betInDb.Conditions.ElementAt(0);
        //    var User2InDb = betInDb.Conditions.ElementAt(1);

        //    //TODO: Change this ViewModel
        //    var MyBetsViewModel = new MyBetsViewModel
        //    {
        //        BetFee = betInDb.BetFee,
        //        BetPremise = betInDb.BetPremise,
        //        User1 = User1InDb.UserEmail,
        //        User1Condition = User1InDb.WinCondition,
        //        User2 = User2InDb.UserEmail,
        //        User2Condition = User2InDb.WinCondition,
        //        EndDate = betInDb.EndTime,
        //        BetStatus = Enum.GetName(typeof(BetStatus), betInDb.BetStatus)
        //    };

        //    return Ok(MyBetsViewModel);
        //}

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
