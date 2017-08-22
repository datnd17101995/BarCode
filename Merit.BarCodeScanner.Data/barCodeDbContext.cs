using System.Data.Entity;
using Merit.BarCodeScanner.Models;

namespace Merit.BarCodeScanner.Data
{
    public partial class barCodeDbContext : DbContext
    {
        public barCodeDbContext()
            : base("DBConnectionString")
        {
            //Database.SetInitializer<barCodeDbContext>(new DropCreateDatabaseIfModelChanges<barCodeDbContext>());
        }

        public virtual DbSet<Block> Blocks { get; set; }

        public virtual DbSet<DeliveryBlock> DeliveryBlocks { get; set; }
                
        public virtual DbSet<PalletDetail> PalletDetails { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
    }
}
