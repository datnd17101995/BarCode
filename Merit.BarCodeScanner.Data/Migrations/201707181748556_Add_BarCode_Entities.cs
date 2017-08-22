namespace Merit.BarCodeScanner.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_BarCode_Entities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Blocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BlockId = c.Guid(nullable: false),
                        CreatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DeliveryBlocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BlockId = c.Guid(nullable: false),
                        DestinationId = c.String(),
                        EmployeeId = c.String(),
                        PalletId = c.String(),
                        BlockStartTime = c.DateTime(),
                        BlockEndTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PalletDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PalletId = c.String(nullable: false),
                        EmployeeId = c.String(),
                        Day = c.String(),
                        BlockId = c.Guid(nullable: false),
                        PalletScanTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PalletDetails");
            DropTable("dbo.DeliveryBlocks");
            DropTable("dbo.Blocks");
        }
    }
}
