using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Order.Infrastructure.Entities;

namespace Order.Infrastructure.Context;

public partial class OrderDbContext : DbContext
{
    public OrderDbContext()
    {
    }

    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//         => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=OrderDb;User Id=sa;Password=Sambit@123;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Purchase__3214EC07ADC800B9");

            entity.Property(e => e.AddressLine).HasMaxLength(200);
            entity.Property(e => e.CVV).HasMaxLength(10);
            entity.Property(e => e.CardName).HasMaxLength(50);
            entity.Property(e => e.CardNumber).HasMaxLength(20);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasDefaultValue("system");
            entity.Property(e => e.EmailAddress).HasMaxLength(100);
            entity.Property(e => e.Expiration).HasMaxLength(10);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastModifiedBy)
                .HasMaxLength(100)
                .HasDefaultValue("system");
            entity.Property(e => e.LastModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.ZipCode).HasMaxLength(20);
            entity.Property(e => e.PaymentMethod);

        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
