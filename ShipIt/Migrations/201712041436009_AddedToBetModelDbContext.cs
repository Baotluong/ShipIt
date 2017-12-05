namespace ShipIt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedToBetModelDbContext : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.BetApplicationUsers", newName: "ApplicationUserBets");
            DropPrimaryKey("dbo.ApplicationUserBets");
            AddPrimaryKey("dbo.ApplicationUserBets", new[] { "ApplicationUser_Id", "Bet_Id" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.ApplicationUserBets");
            AddPrimaryKey("dbo.ApplicationUserBets", new[] { "Bet_Id", "ApplicationUser_Id" });
            RenameTable(name: "dbo.ApplicationUserBets", newName: "BetApplicationUsers");
        }
    }
}
