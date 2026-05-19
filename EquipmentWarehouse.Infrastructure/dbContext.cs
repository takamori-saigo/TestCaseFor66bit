using Microsoft.EntityFrameworkCore;
using EquipmentWarehouse.Domain;


namespace EquipmentWarehouse.Infrastructure;

public class dbContext: DbContext
{
    public DbSet<EquipmentType> EquipmentTypes => Set<EquipmentType>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Supplier> Supplier => Set<Supplier>();
    public DbSet<WareHouseItem> WareHouseItem => Set<WareHouseItem>();
    public dbContext(DbContextOptions<dbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(dbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var e in ChangeTracker.Entries<CommonEntity>())
        {
            switch (e.State)
            {
                case EntityState.Added:
                    e.Entity.CreatedDate = now;
                    e.Entity.ModifiedDate = now;
                    break;
                case EntityState.Modified:
                    e.Entity.ModifiedDate = now;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}