using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ShipIt.Models;

namespace ShipIt.ViewModels
{
    public class NewBetViewModel
    {
        //finish validation. not done. figure out how to make sure it valid datetime.
        [Display(Name = "When does the bet end? (Optional)")]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        //[Range(typeof(DateTime), , "2004-12-31",
        //ErrorMessage = "Value for {0} must be between {1} and {2}")]
        //[ValidEndDate(ErrorMessage ="Invalid Date")]
        public DateTime? EndTime { get; set; }

        [Required(ErrorMessage = "Please enter a valid wager.")]
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