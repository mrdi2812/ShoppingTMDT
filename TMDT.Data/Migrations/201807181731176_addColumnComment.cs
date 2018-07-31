namespace TMDT.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addColumnComment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AttachImage = c.String(maxLength: 500),
                        CreateDate = c.DateTime(nullable: false),
                        CreateBy = c.String(maxLength: 250),
                        ModifiedDate = c.DateTime(),
                        Content = c.String(maxLength: 500),
                        ParentId = c.String(),
                        ReplyCount = c.Int(nullable: false),
                        Pings = c.String(maxLength: 500),
                        Status = c.Boolean(nullable: false),
                        PostId = c.Int(),
                        ProductId = c.Int(),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AppUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.CommentVotes",
                c => new
                    {
                        CommentId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.CommentId, t.UserId })
                .ForeignKey("dbo.AppUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Comments", t => t.CommentId, cascadeDelete: true)
                .Index(t => t.CommentId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CommentVotes", "CommentId", "dbo.Comments");
            DropForeignKey("dbo.CommentVotes", "UserId", "dbo.AppUsers");
            DropForeignKey("dbo.Comments", "UserId", "dbo.AppUsers");
            DropIndex("dbo.CommentVotes", new[] { "UserId" });
            DropIndex("dbo.CommentVotes", new[] { "CommentId" });
            DropIndex("dbo.Comments", new[] { "UserId" });
            DropTable("dbo.CommentVotes");
            DropTable("dbo.Comments");
        }
    }
}
