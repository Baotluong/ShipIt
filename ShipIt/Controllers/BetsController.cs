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

        public ActionResult MyBets()
        {
            return View();
        }

        public ActionResult Details(string id)
        {
            var betInDb = _context.Bets.Where(b => b.Id.ToString() == id).SingleOrDefault();

            if (betInDb == null)
                return HttpNotFound();

            var User1InDb = betInDb.Conditions.ElementAt(0);
            var User2InDb = betInDb.Conditions.ElementAt(1);

            var MyBetsViewModel = new MyBetsViewModel
            {
                BetFee = betInDb.BetFee,
                BetPremise = betInDb.BetPremise,
                User1 = User1InDb.UserEmail,
                User1Condition = User1InDb.WinCondition,
                User2 = User2InDb.UserEmail,
                User2Condition = User2InDb.WinCondition,
                EndDate = betInDb.EndTime,
                BetStatus = Enum.GetName(typeof(BetStatus), betInDb.BetStatus)
            };

            return View(MyBetsViewModel);
        }

        public ActionResult New()
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = _context.Users.First(u => u.Id == currentUserId);

            //TODO: why isnt this just NewBetViewModel?
            var viewModel = new NewBetViewModel
            {
                CurrentUserEmail = currentUser.Email
            };

            return View("BetForm", viewModel);
        }

        [HttpPost]
        public ActionResult Save(NewBetViewModel newBetViewModel)
        {
            string currentUserId = User.Identity.GetUserId();

            if (!ModelState.IsValid)
            {
                var viewModel = new NewBetViewModel
                {
                    CurrentUserEmail = _context.Users.Where(u => u.Id == currentUserId).SingleOrDefault().Email
                };
                return View("BetForm", viewModel);
            }

            var newBet = new Bet();

            var newBetUserConditions= new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>(newBetViewModel.User1, newBetViewModel.User1Condition),
                new KeyValuePair<string, string>(newBetViewModel.User2, newBetViewModel.User2Condition)
            };

            List<ApplicationUser> UsersInDb = new List<ApplicationUser>();
            List<Condition> NewBetConditions = new List<Condition>();

            foreach (KeyValuePair<string, string> userConditions in newBetUserConditions)
            {
                var UserinDb = _context.Users
                    .Where(u => u.Email == userConditions.Key).SingleOrDefault();

                if (UserinDb != null)
                    UsersInDb.Add(UserinDb);

                var newCondition = new Condition()
                {
                    WinCondition = userConditions.Value,
                    Bet = newBet,
                    //Another Hack. Talk to Bryce
                    //ApplicationUser = (UserinDb == null) ? null : UserinDb,
                    UserEmail = userConditions.Key,
                    BetStatus = BetStatus.Proposed
                };
                NewBetConditions.Add(newCondition);
            }

            newBet.StartDate = DateTime.Now;
            newBet.BetStatus = BetStatus.Proposed;
            newBet.EndTime = newBetViewModel.EndTime;
            newBet.BetFee = newBetViewModel.BetFee;
            newBet.ApplicationUsers = UsersInDb;
            newBet.BetPremise = newBetViewModel.BetPremise;
            newBet.BetCreatorId = _context.Users.Where(u => u.Id == currentUserId).SingleOrDefault().Id;
            newBet.Conditions = NewBetConditions;

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
            msg.To.Add(new MailAddress("baotluong@gmail.com"));

            msg.Subject = "This is a test Email subject";
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
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = _context.Users.First(u => u.Id == currentUserId);

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
                //condition.Bet.ApplicationUsers.Add(user);
            }
            _context.SaveChanges();
            return RedirectToAction("MyBets", "Bets");
        }
    }
}