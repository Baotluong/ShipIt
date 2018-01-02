using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipIt.Models
{
    public class BetStatus
    {
        public int Id { get; set; }

        public string StatusName { get; set; }

        public static readonly byte Proposed = 1;

        public static readonly byte InProgress = 2;

        public static readonly byte Completed = 3;

        public static readonly byte Paid = 4;
    }
}