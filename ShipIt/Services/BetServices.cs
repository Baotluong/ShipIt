using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ShipIt.Models;
using ShipIt.ViewModels;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Net.Mail;
using System.IO;
using System.Configuration;
using ShipIt.Services;
using System.Web;

namespace ShipIt.Services
{
    public class BetServices
    {
        private ApplicationDbContext _context;

        public BetServices()
        {
            _context = new ApplicationDbContext();
        }

        // DO I need this?
        //protected override void Dispose(bool disposing)
        //{
        //    _context.Dispose();
        //}

        public static string GetUserBetStatusMessage(UserBetStatus userBetStatus)
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

        private ApplicationUser GetCurrentUser()
        {
            string currentUserId = HttpContext.Current.User.Identity.GetUserId();
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
    }
}