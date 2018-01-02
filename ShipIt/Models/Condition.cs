using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShipIt.Models
{
    public class Condition
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string WinCondition { get; set; }

        public Bet Bet { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public BetStatus BetStatus { get; set; }
    }
}