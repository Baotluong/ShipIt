namespace ShipIt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedCondtions : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Conditions", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Conditions", new[] { "ApplicationUser_Id" });
            AddColumn("dbo.Conditions", "UserEmail", c => c.String());
            DropColumn("dbo.Conditions", "ApplicationUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Conditions", "ApplicationUser_Id", c => c.String(maxLength: 128));
            DropColumn("dbo.Conditions", "UserEmail");
            CreateIndex("dbo.Conditions", "ApplicationUser_Id");
            AddForeignKey("dbo.Conditions", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
