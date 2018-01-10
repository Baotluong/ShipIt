namespace ShipIt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBetWinnerAndProposedBetWinnerToBetsAndChangedBetFeeToString : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bets", "ProposedBetWinner", c => c.String());
            AddColumn("dbo.Bets", "BetWinner", c => c.String());
            AlterColumn("dbo.Bets", "BetFee", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Bets", "BetFee", c => c.Single(nullable: false));
            DropColumn("dbo.Bets", "BetWinner");
            DropColumn("dbo.Bets", "ProposedBetWinner");
        }
    }
}
