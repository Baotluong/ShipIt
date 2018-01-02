namespace ShipIt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBetStatusToConditions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Conditions", "BetStatus_Id", c => c.Int());
            CreateIndex("dbo.Conditions", "BetStatus_Id");
            AddForeignKey("dbo.Conditions", "BetStatus_Id", "dbo.BetStatus", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Conditions", "BetStatus_Id", "dbo.BetStatus");
            DropIndex("dbo.Conditions", new[] { "BetStatus_Id" });
            DropColumn("dbo.Conditions", "BetStatus_Id");
        }
    }
}
