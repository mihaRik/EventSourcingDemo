using EventSourcingDemo.ReadModel.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingDemo.ReadModel.DatabaseContext
{
    public class EventSourcingReadModelDbContext : DbContext
    {
        public EventSourcingReadModelDbContext(DbContextOptions<EventSourcingReadModelDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WarehouseReadModel>()
                        .HasMany(_ => _.Items)
                        .WithOne(_ => _.Warehouse)
                        .HasForeignKey(_ => _.WarehouseId);

            modelBuilder.Entity<WarehouseItemReadModel>()
                        .HasOne(_ => _.Product)
                        .WithMany(_ => _.WarehouseItems)
                        .HasForeignKey(_ => _.ProductId);
        }

        public DbSet<WarehouseReadModel> Warehouses { get; set; }
        public DbSet<WarehouseItemReadModel> WarehouseItems { get; set; }
        public DbSet<ProductReadModel> BaseItems { get; set; }
    }
}
