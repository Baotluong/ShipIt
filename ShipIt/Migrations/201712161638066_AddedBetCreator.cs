namespace ShipIt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBetCreator : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApplicationUserBets", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserBets", "Bet_Id", "dbo.Bets");
            DropIndex("dbo.ApplicationUserBets", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserBets", new[] { "Bet_Id" });
            AddColumn("dbo.Bets", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Bets", "BetCreator_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUsers", "Bet_Id", c => c.Guid());
            CreateIndex("dbo.Bets", "ApplicationUser_Id");
            CreateIndex("dbo.Bets", "BetCreator_Id");
            CreateIndex("dbo.AspNetUsers", "Bet_Id");
            AddForeignKey("dbo.Bets", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetUsers", "Bet_Id", "dbo.Bets", "Id");
            AddForeignKey("dbo.Bets", "BetCreator_Id", "dbo.AspNetUsers", "Id");
            DropTable("dbo.ApplicationUserBets");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ApplicationUserBets",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Bet_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Bet_Id });
            
            DropForeignKey("dbo.Bets", "BetCreator_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "Bet_Id", "dbo.Bets");
            DropForeignKey("dbo.Bets", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetUsers", new[] { "Bet_Id" });
            DropIndex("dbo.Bets", new[] { "BetCreator_Id" });
            DropIndex("dbo.Bets", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.AspNetUsers", "Bet_Id");
            DropColumn("dbo.Bets", "BetCreator_Id");
            DropColumn("dbo.Bets", "ApplicationUser_Id");
            CreateIndex("dbo.ApplicationUserBets", "Bet_Id");
            CreateIndex("dbo.ApplicationUserBets", "ApplicationUser_Id");
            AddForeignKey("dbo.ApplicationUserBets", "Bet_Id", "dbo.Bets", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationUserBets", "ApplicationUser_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
