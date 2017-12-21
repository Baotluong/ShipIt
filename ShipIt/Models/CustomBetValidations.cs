using System;
using System.ComponentModel.DataAnnotations;
using ShipIt.ViewModels;

namespace ShipIt.Models
{
    public class ValidEndDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
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