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


namespace ShipIt.Services
{
    public class BetServices
    {
        private ApplicationDbContext _context;

        public BetServices()
        {
            _context = new ApplicationDbContext();
        }

        public static string GetUserBetStatusMessage(UserBetStatus userBetStatus)
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
    }
}