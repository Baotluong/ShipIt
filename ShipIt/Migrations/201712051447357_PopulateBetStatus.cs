namespace ShipIt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateBetStatus : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO BetStatus (StatusName) VALUES ('Proposed')");
            Sql("INSERT INTO BetStatus (StatusName) VALUES ('In Progress')");
            Sql("INSERT INTO BetStatus (StatusName) VALUES ('Completed')");
            Sql("INSERT INTO BetStatus (StatusName) VALUES ('Paid')");
        }
        
        public override void Down()
        {
        }
    }
}
