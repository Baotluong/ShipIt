using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipIt.Models
{
    public enum UserBetStatus
    {
        ProposedBet = 1,
        CanAcceptBet = 2,
        CanProposeWinner = 3,
        ProposedWinner = 4,
        CanAcceptWinner = 5,
        NeedsToSettle = 6,
        CanAcceptPaid = 7,
        Resolved = 8
    }
}