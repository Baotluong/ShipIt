using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipIt.Models
{
    public class Bet
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndTime { get; set; }

        public float BetFee { get; set; }

        public BetStatus BetStatus { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}