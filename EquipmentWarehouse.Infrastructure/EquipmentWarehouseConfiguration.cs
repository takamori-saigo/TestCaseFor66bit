using EquipmentWarehouse.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentWarehouse.Infrastructure;

public class EquipmentWarehouseConfiguration: IEntityTypeConfiguration<WareHouseItem>
{
    public void Configure(EntityTypeBuilder<WareHouseItem> builder)
    {
        builder.HasIndex(x => x.Model);
        builder.HasIndex(x => x.Price);

        builder.HasOne(x => x.EquipmentType)
            .WithMany(x => x.WareHouseItems)
            .HasForeignKey(x => x.TypeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.Supplier)
            .WithMany(x => x.WareHouseItems)
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.CountryOfItem)
            .WithMany(x => x.WarehouseItems)
            .HasForeignKey(x => x.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.RowVersion)
            .IsConcurrencyToken();
    }
}