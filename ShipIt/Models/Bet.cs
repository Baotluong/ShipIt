using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShipIt.Models
{
    public class Bet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public DateTime StartDate { get; set; }

        [Display(Name = "When will the bet end?")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date.")]
        public DateTime? EndTime { get; set; }

        [Required]
        [Display(Name = "Bet Amount")]
        public float BetFee { get; set; }

        public BetStatus BetStatus { get; set; }

        public string BetCreatorId { get; set; }

        public string BetPremise { get; set; }

        [Required]
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }

        [Required]
        public virtual ICollection<Condition> Conditions { get; set; }

    }
}