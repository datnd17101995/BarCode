namespace Merit.BarCodeScanner.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Table_BlockShift : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BlockShifts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BlockId = c.Guid(nullable: false),
                        ShiftId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BlockShifts");
        }
    }
}
