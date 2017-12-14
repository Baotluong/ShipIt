using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipIt.ViewModels
{
    public class NewBetViewModel
    {
        public DateTime EndTime { get; set; }

        public float BetFee { get; set; }

        public string User1 { get; set; }

        public string User2 { get; set; }
    }
}