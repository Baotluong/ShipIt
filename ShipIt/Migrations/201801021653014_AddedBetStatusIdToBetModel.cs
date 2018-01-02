namespace ShipIt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBetStatusIdToBetModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bets", "BetStatus_Id", "dbo.BetStatus");
            DropIndex("dbo.Bets", new[] { "BetStatus_Id" });
            RenameColumn(table: "dbo.Bets", name: "BetStatus_Id", newName: "BetStatusId");
            AlterColumn("dbo.Bets", "BetStatusId", c => c.Int(nullable: false));
            CreateIndex("dbo.Bets", "BetStatusId");
            AddForeignKey("dbo.Bets", "BetStatusId", "dbo.BetStatus", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bets", "BetStatusId", "dbo.BetStatus");
            DropIndex("dbo.Bets", new[] { "BetStatusId" });
            AlterColumn("dbo.Bets", "BetStatusId", c => c.Int());
            RenameColumn(table: "dbo.Bets", name: "BetStatusId", newName: "BetStatus_Id");
            CreateIndex("dbo.Bets", "BetStatus_Id");
            AddForeignKey("dbo.Bets", "BetStatus_Id", "dbo.BetStatus", "Id");
        }
    }
}
