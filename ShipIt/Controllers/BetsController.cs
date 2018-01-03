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
            var bet = _context.Bets
                .Include(b => b.ApplicationUsers)
                .Include(b => b.Conditions)
                .SingleOrDefault(b => b.Id.ToString() == id);

            if (bet == null)
            {
                return HttpNotFound();
            }

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

            return View(NewBetViewModel);
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
            string currentUserEmail = _context.Users.Where(u => u.Id == currentUserId).SingleOrDefault().Email;

            if (!ModelState.IsValid)
            {
                var viewModel = new NewBetViewModel
                {
                    CurrentUserEmail = currentUserEmail
                };
                return View("BetForm", viewModel);
            }

            var newBet = new Bet();

            var newBetUserConditions= new List<KeyValuePair<string, string>>() {
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
            newBet.BetStatusId = 1;
            newBet.EndTime = newBetViewModel.EndTime;
            newBet.BetFee = newBetViewModel.BetFee;
            newBet.ApplicationUsers = UsersInDb;
            newBet.BetPremise = newBetViewModel.BetPremise;
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
                    ApplicationUser = UserinDb,
                };
                NewBetConditions.Add(newCondition);
            }
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
                new System.Net.NetworkCredential("baotluong@gmail.com", "password");
            client.UseDefaultCredentials = false;
            client.Credentials = credentials;

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("baotluong@gmail.com");
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

            return RedirectToAction("Index", "Bets");
        }
    }
}