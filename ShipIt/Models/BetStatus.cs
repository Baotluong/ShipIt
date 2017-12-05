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

        public static readonly byte Proposed = 0;

        public static readonly byte InProgress = 1;

        public static readonly byte Completed = 2;

        public static readonly byte Paid = 3;
    }
}