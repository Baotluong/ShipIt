using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipIt.ViewModels
{
    public class BetsDetailViewModel
    {
        public string BetId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string BetFee { get; set; }

        public string User1 { get; set; }

        public string User2 { get; set; }

        public string User1Condition { get; set; }

        public string User2Condition { get; set; }

        public string BetPremise { get; set; }

        public string BetStatus { get; set; }

        public string UserBetStatus { get; set; }

        public string UserBetStatusMessage { get; set; }

        public string currentUserEmail { get; set; }
        
        public string ProposedBetWinner { get; set; }

        public string BetWinner { get; set; }

        public string ErrorMessage { get; set; }
    }
}