namespace ShipIt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedBetDateToNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Bets", "EndTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Bets", "EndTime", c => c.DateTime(nullable: false));
        }
    }
}
