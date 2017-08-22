namespace Merit.BarCodeScanner.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change_Key_TableBlock : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Blocks");
            AddPrimaryKey("dbo.Blocks", "BlockId");
            DropColumn("dbo.Blocks", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Blocks", "Id", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.Blocks");
            AddPrimaryKey("dbo.Blocks", "Id");
        }
    }
}
