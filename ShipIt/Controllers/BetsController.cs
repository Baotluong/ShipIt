using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ShipIt.Models;
using ShipIt.ViewModels;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Net.Mail;

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

        public ActionResult BetsIndex(string id)
        {
            var viewModel = new BetsIndexViewModel
            {
                id = id
            };

            return View(viewModel);
        }

        private ApplicationUser GetCurrentUser()
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = _context.Users.Single(u => u.Id == currentUserId);

            return currentUser;
            //compressing the lines to below does not work. Not sure why.
            //return _context.Users.Single(u => u.Id == User.Identity.GetUserId());
        }

        private Bet GetBet(string betId)
        {
            return _context.Bets
                .Include(b => b.Conditions)
                .Include(b => b.ApplicationUsers)
                .Single(b => b.Id.ToString() == betId);
        }

        public string GetUserBetStatusMessage(UserBetStatus userBetStatus)
        {
            var userBetStatusMessage = new Dictionary<UserBetStatus, string> {
                { UserBetStatus.WaitingForAcceptBet, "Waiting for other(s) to accept Bet."},
                { UserBetStatus.CanAcceptBet, "May accept Bet."},
                { UserBetStatus.CanProposeWinner, "May propose Winner."},
                { UserBetStatus.WaitingForAcceptWinner, "Waiting for other(s) to accept Winner."},
                { UserBetStatus.CanAcceptWinner, "May accept Winner"},
                { UserBetStatus.NeedsToSettle, "Waiting for other(s) to accept Settlement."},
                { UserBetStatus.CanAcceptPaid, "May accept Settlement?"},
                { UserBetStatus.Resolved, "Bet is Resolved."},
            };

            return userBetStatusMessage[userBetStatus];
        }

        public ActionResult Details(string id, string errorMessage)
        {
            var betInDb = GetBet(id);
            var currentUser = GetCurrentUser();

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
                BetWager = betInDb.BetWager,
                BetPremise = betInDb.BetPremise,
                User1 = user1Conditions.UserEmail,
                User1Condition = user1Conditions.WinCondition,
                User2 = user2Conditions.UserEmail,
                User2Condition = user2Conditions.WinCondition,
                EndDate = betInDb.EndTime,
                BetStatus = Enum.GetName(typeof(BetStatus), betInDb.BetStatus),
                UserBetStatus = (myConditions != null) ? Enum.GetName(typeof(UserBetStatus), myConditions.UserBetStatus) : "NotUsersBet",
                UserBetStatusMessage = (myConditions != null) ? GetUserBetStatusMessage(myConditions.UserBetStatus) : "NotUsersBets",
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
                CurrentUserEmail = GetCurrentUser().Email
            };

            return View("BetForm", viewModel);
        }

        [HttpPost]
        public ActionResult Save(NewBetViewModel newBetViewModel)
        {
            var currentUser = GetCurrentUser();

            if (!ModelState.IsValid)
            {
                var viewModel = new NewBetViewModel
                {
                    CurrentUserEmail = currentUser.Email
                };
                return View("BetForm", viewModel);
            }

            var newBet = new Bet();

            var newBetUserConditions= new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>(newBetViewModel.User1, newBetViewModel.User1Condition),
                new KeyValuePair<string, string>(newBetViewModel.User2, newBetViewModel.User2Condition)
            };

            List<ApplicationUser> getUsersToBeAddedToBet = new List<ApplicationUser>();
            List<Condition> newConditionsToBeAddedToBet = new List<Condition>();

            foreach (KeyValuePair<string, string> userConditions in newBetUserConditions)
            {
                var getUserInDb = _context.Users
                    .Where(u => u.Email == userConditions.Key).SingleOrDefault();

                if (getUserInDb != null)
                    getUsersToBeAddedToBet.Add(getUserInDb);

                var newCondition = new Condition()
                {
                    WinCondition = userConditions.Value,
                    Bet = newBet,
                    //Another Hack. Talk to Bryce
                    //ApplicationUser = (UserinDb == null) ? null : UserinDb,
                    UserEmail = userConditions.Key,
                    UserBetStatus = (currentUser.Email == userConditions.Key) ? UserBetStatus.WaitingForAcceptBet : UserBetStatus.CanAcceptBet, 
                };
                newConditionsToBeAddedToBet.Add(newCondition);
            }

            newBet.StartDate = DateTime.Now;
            newBet.BetStatus = BetStatus.Proposed;
            newBet.EndTime = newBetViewModel.EndTime;
            newBet.BetWager = newBetViewModel.BetWager;
            newBet.ApplicationUsers = getUsersToBeAddedToBet;
            newBet.BetPremise = newBetViewModel.BetPremise;
            newBet.BetCreatorId = currentUser.Id;
            newBet.Conditions = newConditionsToBeAddedToBet;

            _context.Bets.Add(newBet);
            _context.SaveChanges();

            return RedirectToAction("MyBets", "Bets");
        }

        public ActionResult SendEmail(object sender, EventArgs e)
        {
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;

            // setup Smtp authentication
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential(System.Web.Configuration.WebConfigurationManager.AppSettings["EmailUserName"],
                System.Web.Configuration.WebConfigurationManager.AppSettings["EmailPassword"]);
            client.UseDefaultCredentials = false;
            client.Credentials = credentials;

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("baosapp@gmail.com");
            msg.To.Add(new MailAddress("baosapp@mail.usf.edu"));

            msg.Subject = "This is a test Email subject, bitch";
            msg.IsBodyHtml = true;
            msg.Body = string.Format("<html><head></head><body><b>Test HTML Email</b></body>");

            try
            {
                client.Send(msg);
                //lblMsg.Text = "Your message has been successfully sent.";
            }
            catch (Exception ex)
            {
                //lblMsg.ForeColor = Color.Red;
                //lblMsg.Text = "Error occured while sending your message." + ex.Message;
            }

            return RedirectToAction("MyBets", "Bets");
        }

        public ActionResult ClaimBetsAfterRegistering()
        {
            var currentUser = GetCurrentUser();

            var conditionsQuery = _context.Conditions
                .Include(c => c.Bet)
                .Where(c => c.UserEmail == currentUser.Email)
                .ToList();

            foreach (Condition condition in conditionsQuery)
            {
                //Why don't these commented ones work
                //var betInDb = _context.Bets.Single(b => b.Id == condition.Bet.Id).ApplicationUsers;
                //var betInDb = _context.Bets.Where(b => b.Id == condition.Bet.Id).SingleOrDefault();
                condition.Bet.ApplicationUsers.Add(currentUser);

                //betInDb.ApplicationUsers.Add(currentUser);
            }

            _context.SaveChanges();
            return RedirectToAction("MyBets", "Bets");
        }

        public ActionResult AcceptBet(string betId)
        {
            var betInDb = GetBet(betId);

            //Reloads if user is not on expected status
            if(betInDb.Conditions.Single(c => c.UserEmail == GetCurrentUser().Email).UserBetStatus != UserBetStatus.CanAcceptBet)
                 return RedirectToAction("Details", "Bets", new { id = betId, errorMessage = "Please try again." });

            foreach (Condition condition in betInDb.Conditions)
            {
                condition.UserBetStatus = UserBetStatus.CanProposeWinner;
            }

            betInDb.BetStatus = BetStatus.Active;

            _context.SaveChanges();

            return RedirectToAction("Details", "Bets", new { id = betId });
        }

        public ActionResult ProposeWinner(string proposedWinner, string betId)
        {
            var betInDb = GetBet(betId);

            //Catches if the user is on the right status
            if (betInDb.Conditions.Single(c => c.UserEmail == GetCurrentUser().Email).UserBetStatus != UserBetStatus.CanProposeWinner)
                return RedirectToAction("Details", "Bets", new { id = betId, errorMessage = "Please try again." });

            betInDb.ProposedBetWinner = proposedWinner;

            foreach(Condition condition in betInDb.Conditions)
            {
                condition.UserBetStatus = (condition.UserEmail == proposedWinner) ? UserBetStatus.WaitingForAcceptWinner : UserBetStatus.CanAcceptWinner;
            }

            _context.SaveChanges();

            return RedirectToAction("Details", "Bets", new { id = betId });
        }

        public ActionResult AcceptWinner(string betId)
        {
            var betInDb = GetBet(betId);

            //Catches if the user is on the right status
            if (betInDb.Conditions.Single(c => c.UserEmail == GetCurrentUser().Email).UserBetStatus != UserBetStatus.CanAcceptWinner)
                return RedirectToAction("Details", "Bets", new { id = betId, errorMessage = "Please try again." });

            betInDb.BetWinner = betInDb.ProposedBetWinner;

            foreach (Condition condition in betInDb.Conditions)
            {
                condition.UserBetStatus = (betInDb.BetWager == null) ? condition.UserBetStatus = UserBetStatus.Resolved: (condition.UserEmail == betInDb.BetWinner) ? UserBetStatus.CanAcceptPaid : UserBetStatus.NeedsToSettle;
                //if (betInDb.BetFee == null)
                //    condition.UserBetStatus = UserBetStatus.Resolved;
                //else
                //    condition.UserBetStatus = (condition.UserEmail == betInDb.BetWinner) ? UserBetStatus.CanAcceptPaid : UserBetStatus.NeedsToSettle;
            }

            betInDb.BetStatus = BetStatus.Completed;

            _context.SaveChanges();

            return RedirectToAction("Details", "Bets", new { id = betId });
        }

        public ActionResult AcceptPaid(string betId)
        {
            var betInDb = GetBet(betId);

            //Catches if the user is on the right status
            if (betInDb.Conditions.Single(c => c.UserEmail == GetCurrentUser().Email).UserBetStatus != UserBetStatus.CanAcceptPaid)
                return RedirectToAction("Details", "Bets", new { id = betId, errorMessage = "Please try again." });

            foreach (Condition condition in betInDb.Conditions)
            {
                condition.UserBetStatus = UserBetStatus.Resolved;
            }

            betInDb.BetStatus = BetStatus.Settled;

            _context.SaveChanges();

            return RedirectToAction("Details", "Bets", new { id = betId });
        }
    }
}