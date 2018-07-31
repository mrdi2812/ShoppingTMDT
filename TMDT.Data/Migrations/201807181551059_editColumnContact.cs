namespace TMDT.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editColumnContact : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ContactDetails", "Lat", c => c.String());
            AlterColumn("dbo.ContactDetails", "Lng", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ContactDetails", "Lng", c => c.Double());
            AlterColumn("dbo.ContactDetails", "Lat", c => c.Double());
        }
    }
}
