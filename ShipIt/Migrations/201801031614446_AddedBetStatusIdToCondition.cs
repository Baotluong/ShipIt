namespace ShipIt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBetStatusIdToCondition : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Conditions", "BetStatus_Id", "dbo.BetStatus");
            DropIndex("dbo.Conditions", new[] { "BetStatus_Id" });
            RenameColumn(table: "dbo.Conditions", name: "BetStatus_Id", newName: "BetStatusId");
            AlterColumn("dbo.Conditions", "BetStatusId", c => c.Int(nullable: false));
            CreateIndex("dbo.Conditions", "BetStatusId");
            AddForeignKey("dbo.Conditions", "BetStatusId", "dbo.BetStatus", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Conditions", "BetStatusId", "dbo.BetStatus");
            DropIndex("dbo.Conditions", new[] { "BetStatusId" });
            AlterColumn("dbo.Conditions", "BetStatusId", c => c.Int());
            RenameColumn(table: "dbo.Conditions", name: "BetStatusId", newName: "BetStatus_Id");
            CreateIndex("dbo.Conditions", "BetStatus_Id");
            AddForeignKey("dbo.Conditions", "BetStatus_Id", "dbo.BetStatus", "Id");
        }
    }
}
