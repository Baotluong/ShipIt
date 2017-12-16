using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShipIt.ViewModels
{
    public class NewBetViewModel
    {
        [Required]
        [Display(Name = "When does the bet end?")]
        public DateTime? EndTime { get; set; }

        [Required]
        [Display(Name = "Bet Wager")]
        public float BetFee { get; set; }

        [Required]
        [Display(Name = "Bettor 1")]
        public string User1 { get; set; }

        [Required]
        [Display(Name = "Bettor 2")]
        public string User2 { get; set; }

        [Required]
        [Display(Name = "Bettor 1 Wins If")]
        public string User1Condition { get; set; }

        [Required]
        [Display(Name = "Bettor 2 Wins If")]
        public string User2Condition { get; set; }
    }
}