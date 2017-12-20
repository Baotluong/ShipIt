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
        [Display(Name = "When does the bet end? (Optional)")]
        //failed validations. ask bryce
        //[DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        //[Range(typeof(DateTime), , "2004-12-31",
        //ErrorMessage = "Value for {0} must be between {1} and {2}")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date, ErrorMessage ="Please enter a valid date.")]
        public DateTime? EndTime { get; set; }

        [Required(ErrorMessage = "Please enter a valid wager.")]
        [Display(Name = "Bet Wager")]
        public float BetFee { get; set; }

        [Required(ErrorMessage = "Please enter a valid Email Address.")]
        [Display(Name = "Bettor 1")]
        [DifferentBettorsValidation]
        [EmailAddress(ErrorMessage = "Please enter a valid Email Address.")]
        public string User1 { get; set; }

        [Required(ErrorMessage = "Please enter a valid Email Address.")]
        [Display(Name = "Bettor 2")]
        [DifferentBettorsValidation]
        [EmailAddress(ErrorMessage = "Please enter a valid Email Address.")]
        public string User2 { get; set; }

        [Required]
        [Display(Name = "Bettor 1 Wins If")]
        public string User1Condition { get; set; }

        [Required]
        [Display(Name = "Bettor 2 Wins If")]
        public string User2Condition { get; set; }
    }
}