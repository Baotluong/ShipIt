﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShipIt.Models;
using Microsoft.AspNet.Identity;
using System.Web.Security;

namespace ShipIt.ViewModels
{
    public class BetFormViewModel
    {
        public IEnumerable<BetStatus> BetStatus { get; set; }

        public NewBetViewModel Bet { get; set; }

        public string CurrentUserEmail { get; set; }
    }
}