﻿using System;
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
        [ValidEndDate]
        public DateTime? EndTime { get; set; }

        [Display(Name = "Bet Wager. (Optional)")]
        public string BetWager { get; set; }

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
        [StringLength(500, ErrorMessage = "This condition exceeds 500 characters.")]
        public string User1Condition { get; set; }

        [Required]
        [Display(Name = "Bettor 2 Wins If")]
        [StringLength(500, ErrorMessage = "This condition exceeds 500 characters.")]
        public string User2Condition { get; set; }

        [Required]
        [Display(Name = "What's the Bet?")]
        [StringLength(500, ErrorMessage = "This field exceeds 500 characters.")]
        public string BetPremise { get; set; }

        public string CurrentUserEmail { get; set; }

        public BetStatus BetStatus { get; set; }
    }
}