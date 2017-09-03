using System.Data.Entity;
using Merit.BarCodeScanner.Models;
using System.Reflection;

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

        public virtual DbSet<BlockShift> BlockShifts { get; set; }

        public virtual DbSet<LocationShift> LocationShifts { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
