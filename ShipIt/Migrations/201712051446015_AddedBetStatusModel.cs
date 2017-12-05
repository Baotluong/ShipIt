namespace ShipIt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBetStatusModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BetStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StatusName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Bets", "BetStatus_Id", c => c.Int());
            CreateIndex("dbo.Bets", "BetStatus_Id");
            AddForeignKey("dbo.Bets", "BetStatus_Id", "dbo.BetStatus", "Id");
            DropColumn("dbo.Bets", "BetStatus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Bets", "BetStatus", c => c.Int(nullable: false));
            DropForeignKey("dbo.Bets", "BetStatus_Id", "dbo.BetStatus");
            DropIndex("dbo.Bets", new[] { "BetStatus_Id" });
            DropColumn("dbo.Bets", "BetStatus_Id");
            DropTable("dbo.BetStatus");
        }
    }
}
