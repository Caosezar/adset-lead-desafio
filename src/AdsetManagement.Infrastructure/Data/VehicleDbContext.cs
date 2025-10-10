using Microsoft.EntityFrameworkCore;
using AdsetManagement.Domain.Entities;

namespace AdsetManagement.Infrastructure.Data;

public class VehicleDbContext : DbContext
{
    public VehicleDbContext(DbContextOptions<VehicleDbContext> options) : base(options) { }
    
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<VehicleImage> VehicleImages { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var vehicle = modelBuilder.Entity<Vehicle>();
        
        vehicle.HasKey(v => v.Id);
        vehicle.Property(v => v.Id).ValueGeneratedOnAdd();
        
        vehicle.Property(v => v.Marca).IsRequired().HasMaxLength(100);
        vehicle.Property(v => v.Modelo).IsRequired().HasMaxLength(100);
        vehicle.Property(v => v.Ano).IsRequired().HasMaxLength(4);
        vehicle.Property(v => v.Placa).IsRequired().HasMaxLength(10);
        vehicle.Property(v => v.Km).HasDefaultValue(0);
        vehicle.Property(v => v.Cor).HasMaxLength(50);
        vehicle.Property(v => v.Preco).HasColumnType("decimal(18,2)").IsRequired();
        
        vehicle.Property(v => v.Imagens)
            .HasConversion(
                images => string.Join(";", images ?? new List<string?>()),
                images => images.Split(";", StringSplitOptions.RemoveEmptyEntries).Cast<string?>().ToList()
            );
        
        vehicle.OwnsOne(v => v.OtherOptions, options =>
        {
            options.Property(o => o.ArCondicionado);
            options.Property(o => o.Alarme);
            options.Property(o => o.Airbag);
            options.Property(o => o.ABS);
        });
            
        vehicle.Property(v => v.PacoteICarros).HasMaxLength(50);
        vehicle.Property(v => v.PacoteWebMotors).HasMaxLength(50);
        
        vehicle.Property(v => v.CreationDate).IsRequired();
        vehicle.Property(v => v.UpdateDate);
        vehicle.Property(v => v.CreateUserId).HasMaxLength(100);
        vehicle.Property(v => v.UpdateUserId).HasMaxLength(100);
        
        vehicle.ToTable("Vehicles");

        modelBuilder.Entity<VehicleImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.ContentType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ImageUrl).HasMaxLength(500).IsRequired();
            
            entity.HasOne(e => e.Vehicle)
                  .WithMany(v => v.VehicleImages)
                  .HasForeignKey(e => e.VehicleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}