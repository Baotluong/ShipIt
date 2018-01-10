using System;
using System.ComponentModel.DataAnnotations;
using ShipIt.Models;

namespace ShipIt.ViewModels
{
    public class MyBetsViewModel
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
    }
}