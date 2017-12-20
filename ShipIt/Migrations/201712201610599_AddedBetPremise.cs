namespace ShipIt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBetPremise : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bets", "BetPremise", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bets", "BetPremise");
        }
    }
}
