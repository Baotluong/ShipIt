namespace ShipIt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedCondtionsAgain : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Conditions", "UserBetStatus", c => c.Int(nullable: false));
            DropColumn("dbo.Conditions", "BetStatus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Conditions", "BetStatus", c => c.Int(nullable: false));
            DropColumn("dbo.Conditions", "UserBetStatus");
        }
    }
}
