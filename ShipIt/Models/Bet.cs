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

        public DateTime? EndTime { get; set; }

        public float BetFee { get; set; }

        public int BetStatusId { get; set; }

        public BetStatus BetStatus { get; set; }

        public string BetCreatorId { get; set; }

        public string BetPremise { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }

        public virtual ICollection<Condition> Conditions { get; set; }

    }
}