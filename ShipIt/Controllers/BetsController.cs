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
        private IEmailService emailService;

        public BetsController()
        {
            betService = new BetService();
            emailService = new EmailService();
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
            return View();
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
                BetWager = (betInDb.BetWager != null) ? betInDb.BetWager : "No wager was made.",
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

        [HttpPost]
        public ActionResult Save(NewBetViewModel newBetViewModel)
        {
            Bet newBet = betService.SaveBet(GetCurrentUserId(), newBetViewModel);
            ApplicationUser currentUser = betService.GetCurrentUser(GetCurrentUserId());

            //Email all other users of bet
            foreach (Condition condition in newBet.Conditions)
            {
                if (condition.UserEmail != currentUser.Email)
                {
                    var emailViewModel = new BetStatusEmailViewModel
                    {
                        RecipientEmail = condition.UserEmail,
                        UserName = condition.UserEmail,
                        Subject = "You've been included in a bet: " + newBet.BetPremise + "!",
                        Title = "You've been included in a bet! ",
                        BetPremise = newBet.BetPremise,
                        User1 = newBet.Conditions.ElementAt(0).UserEmail,
                        User1Condition = newBet.Conditions.ElementAt(0).WinCondition,
                        User2 = newBet.Conditions.ElementAt(1).UserEmail,
                        User2Condition = newBet.Conditions.ElementAt(1).WinCondition,
                        Url = "http://localhost:63907/bets/details/" + newBet.Id.ToString(),
                        Description = newBet.Conditions.ElementAt(0).UserEmail + " wins if " + newBet.Conditions.ElementAt(0).WinCondition
                        + ", and " + newBet.Conditions.ElementAt(1).UserEmail + " wins if " + newBet.Conditions.ElementAt(1).WinCondition + ".",
                    };

                    emailService.BetStatusFormatEmail(emailViewModel);
                }
            }

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