using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ShipIt.Models;

namespace ShipIt.ViewModels
{
    public class BetsIndexViewModel
    {
        public string BetId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string BetWager { get; set; }

        public string User1 { get; set; }

        public string User2 { get; set; }

        public string User1Condition { get; set; }

        public string User2Condition { get; set; }

        public string BetPremise { get; set; }

        public string BetStatus { get; set; }

        public string BetWinner { get; set; }

        public string LookUpEmail { get; set; }
    }
}