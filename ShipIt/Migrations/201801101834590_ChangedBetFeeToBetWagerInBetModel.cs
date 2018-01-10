namespace ShipIt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedBetFeeToBetWagerInBetModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bets", "BetWager", c => c.String());
            DropColumn("dbo.Bets", "BetFee");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Bets", "BetFee", c => c.String());
            DropColumn("dbo.Bets", "BetWager");
        }
    }
}
