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
        public DateTime? EndTime { get; set; }

        [Required]
        [Display(Name = "Bet Amount")]
        public float BetFee { get; set; }

        public BetStatus BetStatus { get; set; }

        public ApplicationUser BetCreator { get; set; }

        [Required]
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }

        [Required]
        public virtual ICollection<Condition> Conditions { get; set; }

    }
}