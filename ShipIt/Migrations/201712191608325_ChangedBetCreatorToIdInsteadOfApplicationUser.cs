namespace ShipIt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedBetCreatorToIdInsteadOfApplicationUser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bets", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "Bet_Id", "dbo.Bets");
            DropForeignKey("dbo.Bets", "BetCreator_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Bets", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Bets", new[] { "BetCreator_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "Bet_Id" });
            CreateTable(
                "dbo.ApplicationUserBets",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Bet_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Bet_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Bets", t => t.Bet_Id, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Bet_Id);
            
            AddColumn("dbo.Bets", "BetCreatorId", c => c.String());
            DropColumn("dbo.Bets", "ApplicationUser_Id");
            DropColumn("dbo.Bets", "BetCreator_Id");
            DropColumn("dbo.AspNetUsers", "Bet_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Bet_Id", c => c.Guid());
            AddColumn("dbo.Bets", "BetCreator_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Bets", "ApplicationUser_Id", c => c.String(maxLength: 128));
            DropForeignKey("dbo.ApplicationUserBets", "Bet_Id", "dbo.Bets");
            DropForeignKey("dbo.ApplicationUserBets", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ApplicationUserBets", new[] { "Bet_Id" });
            DropIndex("dbo.ApplicationUserBets", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Bets", "BetCreatorId");
            DropTable("dbo.ApplicationUserBets");
            CreateIndex("dbo.AspNetUsers", "Bet_Id");
            CreateIndex("dbo.Bets", "BetCreator_Id");
            CreateIndex("dbo.Bets", "ApplicationUser_Id");
            AddForeignKey("dbo.Bets", "BetCreator_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetUsers", "Bet_Id", "dbo.Bets", "Id");
            AddForeignKey("dbo.Bets", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
