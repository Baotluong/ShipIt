using System;
using System.Collections.Generic;
using System.Linq;
using ShipIt.Models;
using ShipIt.ViewModels;
using System.Data.Entity;

namespace ShipIt.Services
{
    public interface IBetService : IDisposable
    {
        ApplicationUser GetCurrentUser(string currentUserId);
        Bet GetBet(string betId);
        string GetUserBetStatusMessage(UserBetStatus userBetStatus);
        Bet SaveBet(string currentUserId, NewBetViewModel newBetViewModel);
        void ClaimBetsAfterRegistering(string currrentUserId);
        void AcceptBet(string currentUserId, string betId);
        void ProposeWinner(string currentUserId, string proposedBetWinner, string betId);
        void AcceptWinner(string currentUserId, string betId);
        void AcceptPaid(string currentUserId, string betId);
    }

    public class BetService : IBetService
    {
        private ApplicationDbContext _context;
        private IEmailService emailService;

        public BetService()
        {
            _context = new ApplicationDbContext();
            emailService = new EmailService();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public string GetUserBetStatusMessage(UserBetStatus userBetStatus)
        {
            var userBetStatusMessage = new Dictionary<UserBetStatus, string> {
                { UserBetStatus.WaitingForAcceptBet, "Waiting for other(s) to accept Bet."},
                { UserBetStatus.CanAcceptBet, "May accept Bet."},
                { UserBetStatus.CanProposeWinner, "May propose Winner."},
                { UserBetStatus.WaitingForAcceptWinner, "Waiting for other(s) to accept Winner."},
                { UserBetStatus.CanAcceptWinner, "May accept Winner"},
                { UserBetStatus.NeedsToSettle, "Please settle Bet with other(s)."},
                { UserBetStatus.CanAcceptPaid, "May accept Settlement."},
                { UserBetStatus.Resolved, "Bet is Resolved."},
            };

            return userBetStatusMessage[userBetStatus];
        }

        public ApplicationUser GetCurrentUser(string currentUserId)
        {
            ApplicationUser currentUser = _context.Users.Single(u => u.Id == currentUserId);

            return currentUser;
            //compressing the lines to below does not work. Not sure why.
            //return _context.Users.Single(u => u.Id == User.Identity.GetUserId());
        }

        public Bet GetBet(string betId)
        {
            return _context.Bets
                .Include(b => b.Conditions)
                .Include(b => b.ApplicationUsers)
                .Single(b => b.Id.ToString() == betId);
        }

        public Bet SaveBet(string currentUserId, NewBetViewModel newBetViewModel)
        {
            var currentUser = GetCurrentUser(currentUserId);

            var betToBeAdded = new Bet();

            var newBetConditions = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>(newBetViewModel.User1, newBetViewModel.User1Condition),
                new KeyValuePair<string, string>(newBetViewModel.User2, newBetViewModel.User2Condition)
            };

            List<ApplicationUser> usersToBeAddedToBet = new List<ApplicationUser>();
            List<Condition> conditionsToBeAddedToBet = new List<Condition>();

            foreach (KeyValuePair<string, string> condition in newBetConditions)
            {
                var getUserInDb = _context.Users
                    .Where(u => u.Email == condition.Key).SingleOrDefault();

                if (getUserInDb != null)
                    usersToBeAddedToBet.Add(getUserInDb);

                var newCondition = new Condition()
                {
                    WinCondition = condition.Value,
                    Bet = betToBeAdded,
                    UserEmail = condition.Key,
                    UserBetStatus = (currentUser.Email == condition.Key) ? UserBetStatus.WaitingForAcceptBet : UserBetStatus.CanAcceptBet,
                };
                conditionsToBeAddedToBet.Add(newCondition);
            }

            betToBeAdded.StartDate = DateTime.Now;
            betToBeAdded.BetStatus = BetStatus.Proposed;
            betToBeAdded.EndTime = newBetViewModel.EndTime;
            betToBeAdded.BetWager = newBetViewModel.BetWager;
            betToBeAdded.ApplicationUsers = usersToBeAddedToBet;
            betToBeAdded.BetPremise = newBetViewModel.BetPremise;
            betToBeAdded.BetCreatorId = currentUser.Id;
            betToBeAdded.Conditions = conditionsToBeAddedToBet;

            _context.Bets.Add(betToBeAdded);
            _context.SaveChanges();

            return betToBeAdded;
        }

        public void ClaimBetsAfterRegistering(string currentUserId)
        {
            var currentUser = GetCurrentUser(currentUserId);

            List<Condition> conditionsQuery = _context.Conditions
                .Include(c => c.Bet)
                .Where(c => c.UserEmail == currentUser.Email)
                .ToList();

            foreach (Condition condition in conditionsQuery)
            {
                condition.Bet.ApplicationUsers.Add(currentUser);
            }

            _context.SaveChanges();
        }

        public void AcceptBet(string currentUserId, string betId)
        {
            var betInDb = GetBet(betId);
            var currentUser = GetCurrentUser(currentUserId);

            //Reloads if user is not on expected status
            if (betInDb.Conditions.Single(c => c.UserEmail == currentUser.Email).UserBetStatus != UserBetStatus.CanAcceptBet)
                throw new InvalidOperationException("Please try again");

            betInDb.Conditions.Single(c => c.UserEmail == currentUser.Email).UserBetStatus = UserBetStatus.WaitingForAcceptBet;

            bool changeStatusToActive = true;
            foreach (Condition condition in betInDb.Conditions)
            {
                if (condition.UserBetStatus == UserBetStatus.CanAcceptBet)
                    changeStatusToActive = false;
            }

            if (changeStatusToActive)
            {
                betInDb.BetStatus = BetStatus.Active;
                foreach (Condition condition in betInDb.Conditions)
                {
                    condition.UserBetStatus = UserBetStatus.CanProposeWinner;

                    //Emails all other users of bet
                    if (condition.UserEmail != currentUser.Email)
                    {
                        var emailObject = new BetStatusEmailViewModel
                        {
                            RecipientEmail = condition.UserEmail,
                            UserName = condition.UserEmail,
                            Subject = betInDb.BetPremise + " is now active!",
                            Title = "This bet is now active!",
                            Description = betInDb.Conditions.ElementAt(0).UserEmail + " and " + betInDb.Conditions.ElementAt(1).UserEmail
                            + " have accepted the terms of the bet. Follow the link below to record the winner after the bet ends!"
                        };
                        LoadBetStatusEmailViewModel(betInDb, currentUser, emailObject);
                        emailService.BetStatusFormatEmail(emailObject);
                    }
                }
            }
            _context.SaveChanges();
        }

        public void ProposeWinner(string currentUserId, string proposedBetWinner, string betId)
        {
            var betInDb = GetBet(betId);
            var currentUser = GetCurrentUser(currentUserId);

            //Catches if the user is on the right status
            if (betInDb.Conditions.Single(c => c.UserEmail == currentUser.Email).UserBetStatus != UserBetStatus.CanProposeWinner &&
                betInDb.Conditions.Single(c => c.UserEmail == currentUser.Email).UserBetStatus != UserBetStatus.CanAcceptWinner)
                throw new InvalidOperationException("Please try again.");

            betInDb.ProposedBetWinner = proposedBetWinner;
            _context.SaveChanges();

            if (betInDb.ProposedBetWinner == currentUser.Email)
                foreach (Condition condition in betInDb.Conditions)
                {
                    condition.UserBetStatus = (condition.UserEmail == proposedBetWinner) ? UserBetStatus.WaitingForAcceptWinner : UserBetStatus.CanAcceptWinner;

                    var emailObject = new BetStatusEmailViewModel
                    {
                        RecipientEmail = condition.UserEmail,
                        UserName = condition.UserEmail,
                        Subject = proposedBetWinner + " has been proposed as the winner of " + betInDb.BetPremise,
                        Title = "Has " + proposedBetWinner + " won " + betInDb.BetPremise + "?",
                        Description = currentUser.Email + " has proposed that " + proposedBetWinner
                        + " won this bet. Follow the link below to accept this result or propose a different result!"
                    };
                    LoadBetStatusEmailViewModel(betInDb, currentUser, emailObject);
                    emailService.BetStatusFormatEmail(emailObject);
                }
            else
                AcceptWinner(currentUserId, betId);
        }

        public void AcceptWinner(string currentUserId, string betId)
        {
            var betInDb = GetBet(betId);
            var currentUser = GetCurrentUser(currentUserId);

            //Catches if the user is on the right status
            if (betInDb.Conditions.Single(c => c.UserEmail == currentUser.Email).UserBetStatus != UserBetStatus.CanAcceptWinner &&
                betInDb.Conditions.Single(c => c.UserEmail == currentUser.Email).UserBetStatus != UserBetStatus.CanProposeWinner)
                throw new InvalidOperationException("Please try again.");

            betInDb.BetWinner = betInDb.ProposedBetWinner;

            foreach (Condition condition in betInDb.Conditions)
            {
                condition.UserBetStatus = (betInDb.BetWager == null) ? 
                    condition.UserBetStatus = UserBetStatus.Resolved : 
                    (condition.UserEmail == betInDb.BetWinner) ? 
                        UserBetStatus.CanAcceptPaid : 
                        UserBetStatus.NeedsToSettle;

                var emailObject = new BetStatusEmailViewModel();
                if (condition.UserBetStatus == UserBetStatus.Resolved)
                {
                    emailObject.RecipientEmail = condition.UserEmail;
                    emailObject.UserName = condition.UserEmail;
                    emailObject.Title = betInDb.ProposedBetWinner + " has won!";
                    emailObject.Subject = betInDb.ProposedBetWinner + " has been declared the winner!";
                    emailObject.Description = "The bet is resolved! No wager was set for this bet.";
                    betInDb.BetStatus = BetStatus.Settled;
                }
                else if (condition.UserBetStatus == UserBetStatus.CanAcceptPaid)
                {
                    emailObject.RecipientEmail = condition.UserEmail;
                    emailObject.UserName = condition.UserEmail;
                    emailObject.Title = betInDb.ProposedBetWinner + " has won!";
                    emailObject.Subject = betInDb.ProposedBetWinner + " has been declared the winner of: " + betInDb.BetPremise;
                    emailObject.Description = "You have won the bet! Update the bet when the wager of: " + betInDb.BetWager + " has been settled.";
                    betInDb.BetStatus = BetStatus.Completed;
                }
                else
                {
                    emailObject.RecipientEmail = condition.UserEmail;
                    emailObject.UserName = condition.UserEmail;
                    emailObject.Title = betInDb.ProposedBetWinner + " has won!";
                    emailObject.Subject = betInDb.ProposedBetWinner + " has been declared the winner of: " + betInDb.BetPremise;
                    emailObject.Description = betInDb.ProposedBetWinner + "has won the bet! Please settle the wager of: " + betInDb.BetWager + " with him/her.";
                    betInDb.BetStatus = BetStatus.Completed;
                }

                LoadBetStatusEmailViewModel(betInDb, currentUser, emailObject);
                emailService.BetStatusFormatEmail(emailObject);
            }
            _context.SaveChanges();
        }

        public void AcceptPaid(string currentUserId, string betId)
        {
            var betInDb = GetBet(betId);
            var currentUser = GetCurrentUser(currentUserId);

            //Catches if the user is on the right status
            if (betInDb.Conditions.Single(c => c.UserEmail == currentUser.Email).UserBetStatus != UserBetStatus.CanAcceptPaid)
                throw new InvalidOperationException("Please try again.");

            foreach (Condition condition in betInDb.Conditions)
            {
                condition.UserBetStatus = UserBetStatus.Resolved;

                var emailObject = new BetStatusEmailViewModel
                {
                    RecipientEmail = condition.UserEmail,
                    UserName = condition.UserEmail,
                    Subject = "The bet has been resolved! " + betInDb.BetWinner + " has won!",
                    Title = "All users have settled the bet!",
                    BetPremise = betInDb.BetPremise,
                    User1 = betInDb.Conditions.ElementAt(0).UserEmail,
                    User1Condition = betInDb.Conditions.ElementAt(0).WinCondition,
                    User2 = betInDb.Conditions.ElementAt(1).UserEmail,
                    User2Condition = betInDb.Conditions.ElementAt(1).WinCondition,
                    Url = "http://localhost:63907/bets/details/" + betInDb.Id.ToString(),
                    Description = "The bet is resolved! All wagers have been settled! Follow the link below to see the details or create a new bet."
                };

                emailService.BetStatusFormatEmail(emailObject);
            }

            betInDb.BetStatus = BetStatus.Settled;

            _context.SaveChanges();
        }

        private BetStatusEmailViewModel LoadBetStatusEmailViewModel(Bet betInDb, ApplicationUser currentUser, BetStatusEmailViewModel betStatusEmailViewModel)
        {
            betStatusEmailViewModel.BetPremise = betInDb.BetPremise;
            betStatusEmailViewModel.User1 = betInDb.Conditions.ElementAt(0).UserEmail;
            betStatusEmailViewModel.User1Condition = betInDb.Conditions.ElementAt(0).WinCondition;
            betStatusEmailViewModel.User2 = betInDb.Conditions.ElementAt(1).UserEmail;
            betStatusEmailViewModel.User2Condition = betInDb.Conditions.ElementAt(1).WinCondition;
            betStatusEmailViewModel.Url = "http://localhost:63907/bets/details/" + betInDb.Id.ToString();

            return betStatusEmailViewModel;
        }
    }
}