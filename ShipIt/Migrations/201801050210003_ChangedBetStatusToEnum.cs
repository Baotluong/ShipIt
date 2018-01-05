namespace ShipIt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedBetStatusToEnum : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bets", "BetStatusId", "dbo.BetStatus");
            DropForeignKey("dbo.Conditions", "BetStatusId", "dbo.BetStatus");
            DropIndex("dbo.Bets", new[] { "BetStatusId" });
            DropIndex("dbo.Conditions", new[] { "BetStatusId" });
            AddColumn("dbo.Bets", "BetStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Conditions", "BetStatus", c => c.Int(nullable: false));
            DropColumn("dbo.Bets", "BetStatusId");
            DropColumn("dbo.Conditions", "BetStatusId");
            DropTable("dbo.BetStatus");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.BetStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StatusName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Conditions", "BetStatusId", c => c.Int(nullable: false));
            AddColumn("dbo.Bets", "BetStatusId", c => c.Int(nullable: false));
            DropColumn("dbo.Conditions", "BetStatus");
            DropColumn("dbo.Bets", "BetStatus");
            CreateIndex("dbo.Conditions", "BetStatusId");
            CreateIndex("dbo.Bets", "BetStatusId");
            AddForeignKey("dbo.Conditions", "BetStatusId", "dbo.BetStatus", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Bets", "BetStatusId", "dbo.BetStatus", "Id", cascadeDelete: true);
        }
    }
}
