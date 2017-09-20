namespace Merit.BarCodeScanner.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Column_DateOfShift_On_Table_BlockShift : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BlockShifts", "DateOfShift", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BlockShifts", "DateOfShift");
        }
    }
}
