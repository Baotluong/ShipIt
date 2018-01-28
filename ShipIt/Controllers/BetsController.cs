using System;
using System.Linq;
using System.Web.Mvc;
using ShipIt.Models;
using ShipIt.ViewModels;
using Microsoft.AspNet.Identity;
using ShipIt.Services;

namespace ShipIt.Controllers
{
    public class BetsController : Controller
    {
        private IBetService betService;
        private IShipItEmailService emailService;

        public BetsController()
        {
            betService = new BetService();
            emailService = new ShipItEmailService();
        }

        protected override void Dispose(bool disposing)
        {
            betService.Dispose();
        }

        public string GetCurrentUserId()
        {
            return User.Identity.GetUserId();
        }

        // GET: Bet
        public ActionResult Index()
        {
            return RedirectToAction("BetsIndex", "Bets");
        }

        //bets/betsindex/{id}
        public ActionResult BetsIndex(string id)
        {
            var viewModel = new BetsIndexViewModel
            {
                LookUpEmail = id
            };

            return View(viewModel);
        }

        public ActionResult Details(string id, string errorMessage)
        {
            var betInDb = betService.GetBet(id);
            var currentUser = betService.GetCurrentUser(GetCurrentUserId());

            if (betInDb == null)
                return HttpNotFound();

            var user1Conditions = betInDb.Conditions.ElementAt(0);
            var user2Conditions = betInDb.Conditions.ElementAt(1);
            var myConditions = new Condition();

            if (currentUser.Email == user1Conditions.UserEmail)
                myConditions = user1Conditions;
            else if (currentUser.Email == user2Conditions.UserEmail)
                myConditions = user2Conditions;
            else
                myConditions = null;

            var betsDetailViewModel = new BetsDetailViewModel
            {
                BetId = betInDb.Id.ToString(),
                BetWager = betInDb.BetWager ?? "No wager was made.",
                BetPremise = betInDb.BetPremise,
                User1 = user1Conditions.UserEmail,
                User1Condition = user1Conditions.WinCondition,
                User2 = user2Conditions.UserEmail,
                User2Condition = user2Conditions.WinCondition,
                StartDate = betInDb.StartDate,
                EndDate = betInDb.EndTime,
                BetStatus = Enum.GetName(typeof(BetStatus), betInDb.BetStatus),
                UserBetStatus = (myConditions != null) ? Enum.GetName(typeof(UserBetStatus), myConditions.UserBetStatus) : "NotUsersBet",
                UserBetStatusMessage = (myConditions != null) ?
                            betService.GetUserBetStatusMessage(myConditions.UserBetStatus) : Enum.GetName(typeof(BetStatus), betInDb.BetStatus),
                currentUserEmail = currentUser.Email,
                BetWinner = betInDb.BetWinner,
                ProposedBetWinner = betInDb.ProposedBetWinner,
                ErrorMessage = errorMessage
            };

            return View(betsDetailViewModel);
        }

        public ActionResult New()
        {
            var viewModel = new NewBetViewModel
            {
                CurrentUserEmail = betService.GetCurrentUser(GetCurrentUserId()).Email
            };

            return View("BetForm", viewModel);
        }

        public ActionResult Delete(string id)
        {
            try
            {
                betService.DeleteBet(GetCurrentUserId(), id);
            }
            catch (InvalidOperationException ex)
            {
                return RedirectToAction("Details", "Bets", new { id = id, errorMessage = ex });
            }

            return RedirectToAction("BetsIndex", "Bets");
        }

        [HttpPost]
        public ActionResult Save(NewBetViewModel newBetViewModel)
        {
            betService.SaveBet(GetCurrentUserId(), newBetViewModel);

            return RedirectToAction("BetsIndex", "Bets");
        }

        public ActionResult ClaimBetsAfterRegistering()
        {
            betService.ClaimBetsAfterRegistering(GetCurrentUserId());

            return RedirectToAction("BetsIndex", "Bets");
        }

        public ActionResult AcceptBet(string betId)
        {
            try
            {
                betService.AcceptBet(GetCurrentUserId(), betId);
            }
            catch(InvalidOperationException ex)
            {
                return RedirectToAction("Details", "Bets", new { id = betId, errorMessage = ex });
            }

            return RedirectToAction("Details", "Bets", new { id = betId });
        }

        public ActionResult ProposeWinner(string proposedBetWinner, string betId)
        {
            try
            {
                betService.ProposeWinner(GetCurrentUserId(), proposedBetWinner, betId);

            } 
            catch(InvalidOperationException ex)
            {
                return RedirectToAction("Details", "Bets", new { id = betId, errorMessage = ex });

            }

            return RedirectToAction("Details", "Bets", new { Id = betId });
        }

        public ActionResult AcceptWinner(string betId)
        {
            try
            {
                betService.AcceptWinner(GetCurrentUserId(), betId);

            }
            catch (InvalidOperationException ex)
            {
                return RedirectToAction("Details", "Bets", new { id = betId, errorMessage = ex });

            }

            return RedirectToAction("Details", "Bets", new { id = betId });
        }

        public ActionResult AcceptPaid(string betId)
        {
            try
            {
                betService.AcceptPaid(GetCurrentUserId(), betId);

            }
            catch (InvalidOperationException ex)
            {
                return RedirectToAction("Details", "Bets", new { id = betId, errorMessage = ex });

            }
            
            return RedirectToAction("Details", "Bets", new { id = betId });
        }
    }
}