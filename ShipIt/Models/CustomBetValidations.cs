using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ShipIt.Models;
using ShipIt.ViewModels;

namespace ShipIt.Models
{
    public class ValidEndDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //var bet = (Bet)validationContext.ObjectInstance;

            //if (customer.MembershipTypeId == MembershipType.Unknown || customer.MembershipTypeId == MembershipType.PayAsYouGo)
            //    return ValidationResult.Success;

            //if (customer.Birthdate == null)
            //    return new ValidationResult("Birthdate is required.");

            //var age = DateTime.Now.Year - customer.Birthdate.Value.Year;

            //return (age >= 18)
            //    ? ValidationResult.Success
            //    : new ValidationResult("Customer must be 18 in order to have a membership");

            DateTime date = Convert.ToDateTime(value);

            return (date >= DateTime.Now.AddDays(-1))
                ? ValidationResult.Success
                : new ValidationResult("End Date must be in the future.");
        }
    }

    public class DifferentBettorsValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var bet = (NewBetViewModel)validationContext.ObjectInstance;

            return (bet.User1 == bet.User2)
                ? new ValidationResult("Bettors must be different.")
                : ValidationResult.Success;
        }
    }
}