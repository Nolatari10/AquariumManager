using AquariumManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Persistence;

public class AquariumDbContext : DbContext
{
    public AquariumDbContext(DbContextOptions<AquariumDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<Species> Species => Set<Species>();
    public DbSet<InventoryLot> InventoryLots => Set<InventoryLot>();
    public DbSet<MortalityRecord> MortalityRecords => Set<MortalityRecord>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();

    public DbSet<Sale> Sales { get; set; } = null!;
    public DbSet<SaleItem> SaleItems { get; set; } = null!;
    public DbSet<User> Users => Set<User>();

    public DbSet<Tank> Tanks => Set<Tank>();
    public DbSet<WaterParameterLog> WaterParameterLogs => Set<WaterParameterLog>();
    public DbSet<MaintenanceLog> MaintenanceLogs => Set<MaintenanceLog>();
    public DbSet<FertilizationLog> FertilizationLogs => Set<FertilizationLog>();
    public DbSet<FertilizerPreset> FertilizerPresets => Set<FertilizerPreset>();
    public DbSet<TankPhoto> TankPhotos => Set<TankPhoto>();
    public DbSet<TargetParameterRange> TargetParameterRanges => Set<TargetParameterRange>();
    public DbSet<SpeciesVariant> SpeciesVariants => Set<SpeciesVariant>();
    public DbSet<AlertConfig> AlertConfigs => Set<AlertConfig>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>(builder =>
        {
            builder.ToTable("Tenants");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
            builder.Property(t => t.ContactInfo).HasMaxLength(500);

            builder.HasMany(t => t.Users)
                   .WithOne(u => u.Tenant)
                   .HasForeignKey(u => u.TenantId)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Species>(builder =>
        {
            builder.ToTable("Species");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.TenantId).IsRequired();
            builder.Property(s => s.CommonName).IsRequired().HasMaxLength(200);
            builder.Property(s => s.ScientificName).HasMaxLength(200);
            builder.Property(s => s.Type).HasMaxLength(100);
            builder.Property(s => s.Variety).HasMaxLength(100);
            builder.Property(s => s.Category).HasMaxLength(100);
            builder.Property(s => s.MinPH).HasPrecision(3, 2);
            builder.Property(s => s.MaxPH).HasPrecision(3, 2);
            builder.Property(s => s.MinTemperature).HasPrecision(4, 1);
            builder.Property(s => s.MaxTemperature).HasPrecision(4, 1);
        });

        modelBuilder.Entity<InventoryLot>(builder =>
        {
            builder.ToTable("InventoryLots");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.TenantId).IsRequired();

            builder.HasOne(l => l.SpeciesVariant)
                   .WithMany(v => v.InventoryLots)
                   .HasForeignKey(l => l.SpeciesVariantId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.Supplier)
                   .WithMany(s => s.InventoryLots)
                   .HasForeignKey(l => l.SupplierId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Property(l => l.UnitCost).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<MortalityRecord>(builder =>
        {
            builder.ToTable("MortalityRecords");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.TenantId).IsRequired();

            builder.HasOne(m => m.InventoryLot)
                   .WithMany(l => l.MortalityRecords)
                   .HasForeignKey(m => m.InventoryLotId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Supplier>(builder =>
        {
            builder.ToTable("Suppliers");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.TenantId).IsRequired();
            builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<User>(builder =>
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.Role).IsRequired().HasMaxLength(50);
            builder.Property(u => u.TenantId).IsRequired();
        });

        modelBuilder.Entity<Tank>(builder =>
        {
            builder.ToTable("Tanks");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.TenantId).IsRequired();
            builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
            builder.Property(t => t.SizeLiters).HasColumnType("decimal(8,1)");
            builder.Property(t => t.TankType).HasConversion<string>().HasMaxLength(50);
            builder.Property(t => t.Substrate).HasMaxLength(100);
            builder.Property(t => t.LightDescription).HasMaxLength(200);
            builder.Property(t => t.FilterDescription).HasMaxLength(200);
            builder.Property(t => t.HeaterSetpointCelsius).HasColumnType("decimal(4,1)");

            builder.HasOne(t => t.OwnerUser)
                   .WithMany()
                   .HasForeignKey(t => t.OwnerUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<WaterParameterLog>(builder =>
        {
            builder.ToTable("WaterParameterLogs");
            builder.HasKey(w => w.Id);
            builder.Property(w => w.TenantId).IsRequired();
            builder.Property(w => w.pH).HasColumnType("decimal(4,2)");
            builder.Property(w => w.TemperatureCelsius).HasColumnType("decimal(4,1)");
            builder.Property(w => w.AmmoniaPpm).HasColumnType("decimal(6,3)");
            builder.Property(w => w.NitritePpm).HasColumnType("decimal(6,3)");
            builder.Property(w => w.NitratePpm).HasColumnType("decimal(6,3)");
            builder.Property(w => w.PhosphatePpm).HasColumnType("decimal(6,3)");
            builder.Property(w => w.PotassiumPpm).HasColumnType("decimal(6,3)");
            builder.Property(w => w.IronPpm).HasColumnType("decimal(6,3)");
            builder.Property(w => w.GeneralHardness).HasColumnType("decimal(5,1)");
            builder.Property(w => w.CarbonateHardness).HasColumnType("decimal(5,1)");
            builder.Property(w => w.Co2Ppm).HasColumnType("decimal(5,1)");
            builder.Property(w => w.SalinityPpt).HasColumnType("decimal(5,2)");
            builder.Property(w => w.Notes).HasMaxLength(500);

            builder.HasOne(w => w.Tank)
                   .WithMany(t => t.WaterParameterLogs)
                   .HasForeignKey(w => w.TankId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MaintenanceLog>(builder =>
        {
            builder.ToTable("MaintenanceLogs");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.TenantId).IsRequired();
            builder.Property(m => m.MaintenanceType).HasConversion<string>().HasMaxLength(50);
            builder.Property(m => m.WaterChangeLiters).HasColumnType("decimal(8,2)");
            builder.Property(m => m.Notes).HasMaxLength(500);

            builder.HasOne(m => m.Tank)
                   .WithMany(t => t.MaintenanceLogs)
                   .HasForeignKey(m => m.TankId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FertilizationLog>(builder =>
        {
            builder.ToTable("FertilizationLogs");
            builder.HasKey(f => f.Id);
            builder.Property(f => f.TenantId).IsRequired();
            builder.Property(f => f.DoseAmount).HasColumnType("decimal(8,2)");
            builder.Property(f => f.DoseUnit).HasConversion<string>().HasMaxLength(10);
            builder.Property(f => f.FertilizerType).HasConversion<string>().HasMaxLength(50);
            builder.Property(f => f.EstimatedNitratePpm).HasColumnType("decimal(6,3)");
            builder.Property(f => f.EstimatedPhosphatePpm).HasColumnType("decimal(6,3)");
            builder.Property(f => f.EstimatedPotassiumPpm).HasColumnType("decimal(6,3)");
            builder.Property(f => f.EstimatedIronPpm).HasColumnType("decimal(6,3)");
            builder.Property(f => f.Notes).HasMaxLength(300);

            builder.HasOne(f => f.Tank)
                   .WithMany(t => t.FertilizationLogs)
                   .HasForeignKey(f => f.TankId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(f => f.FertilizerPreset)
                   .WithMany()
                   .HasForeignKey(f => f.FertilizerPresetId)
                   .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<FertilizerPreset>(builder =>
        {
            builder.ToTable("FertilizerPresets");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.TenantId).IsRequired();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.FertilizerType).HasConversion<string>().HasMaxLength(50);
            builder.Property(p => p.DefaultDoseAmount).HasColumnType("decimal(8,2)");
            builder.Property(p => p.DefaultDoseUnit).HasConversion<string>().HasMaxLength(10);
            builder.Property(p => p.NitratePerDose).HasColumnType("decimal(6,3)");
            builder.Property(p => p.PhosphatePerDose).HasColumnType("decimal(6,3)");
            builder.Property(p => p.PotassiumPerDose).HasColumnType("decimal(6,3)");
            builder.Property(p => p.IronPerDose).HasColumnType("decimal(6,3)");
            builder.Property(p => p.Notes).HasMaxLength(300);

            builder.HasOne(p => p.OwnerUser)
                   .WithMany()
                   .HasForeignKey(p => p.OwnerUserId)
                   .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<TankPhoto>(builder =>
        {
            builder.ToTable("TankPhotos");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.TenantId).IsRequired();
            builder.Property(p => p.ImageUrl).IsRequired().HasMaxLength(500);
            builder.Property(p => p.Caption).HasMaxLength(300);
            builder.Property(p => p.LinkedLogType).HasConversion<string>().HasMaxLength(20);

            builder.HasOne(p => p.Tank)
                   .WithMany(t => t.TankPhotos)
                   .HasForeignKey(p => p.TankId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TargetParameterRange>(builder =>
        {
            builder.ToTable("TargetParameterRanges");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.TenantId).IsRequired();
            builder.Property(r => r.ParameterName).HasConversion<string>().HasMaxLength(30);
            builder.Property(r => r.MinValue).HasColumnType("decimal(8,3)");
            builder.Property(r => r.MaxValue).HasColumnType("decimal(8,3)");
            builder.Property(r => r.Unit).HasMaxLength(10);

            builder.HasOne(r => r.Tank)
                   .WithMany(t => t.TargetParameterRanges)
                   .HasForeignKey(r => r.TankId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SpeciesVariant>(builder =>
        {
            builder.ToTable("SpeciesVariants");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.TenantId).IsRequired();

            builder.HasOne(v => v.Species)
                   .WithMany(s => s.Variants)
                   .HasForeignKey(v => v.SpeciesId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(v => v.VariantName).IsRequired().HasMaxLength(200);

            builder.HasIndex(v => new { v.SpeciesId, v.VariantName }).IsUnique();
        });

        modelBuilder.Entity<AlertConfig>(builder =>
        {
            builder.ToTable("AlertConfigs");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.TenantId).IsRequired();
            builder.Property(a => a.AlertType).IsRequired().HasMaxLength(100);
            builder.Property(a => a.ThresholdValue).HasColumnType("decimal(8,2)");
            builder.HasIndex(a => new { a.TenantId, a.AlertType }).IsUnique();

            builder.HasData(new AlertConfig
            {
                Id = 1,
                AlertType = "HighMortalityRate",
                IsEnabled = true,
                ThresholdValue = 15m,
                TenantId = 1
            });
        });

        modelBuilder.Entity<Sale>(builder =>
        {
            builder.ToTable("Sales");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.TenantId).IsRequired();

            builder.HasMany(s => s.Items)
                   .WithOne(si => si.Sale)
                   .HasForeignKey(si => si.SaleId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SaleItem>(builder =>
        {
            builder.ToTable("SaleItems");
            builder.HasKey(si => si.Id);
            builder.Property(si => si.TenantId).IsRequired();

            builder.HasOne(si => si.Species)
                   .WithMany()
                   .HasForeignKey(si => si.SpeciesId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(si => si.SpeciesVariant)
                   .WithMany()
                   .HasForeignKey(si => si.SpeciesVariantId)
                   .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
