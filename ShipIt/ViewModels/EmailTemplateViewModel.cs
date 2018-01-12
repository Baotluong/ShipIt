using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipIt.ViewModels
{
    public class EmailTemplateViewModel
    {
        public string ToEmail { get; set; }

        public string Subject { get; set; }

        public string UserName { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }

        public string BetPremise { get; set; }

        public string User1 { get; set; }

        public string User1Condition { get; set; }

        public string User2 { get; set; }

        public string User2Condition { get; set; }
    }
}