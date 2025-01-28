using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProductServices.Models;

public partial class ProductApprovalContext : DbContext
{
    public ProductApprovalContext()
    {
    }

    public ProductApprovalContext(DbContextOptions<ProductApprovalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApprovalQueue> ApprovalQueues { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApprovalQueue>(entity =>
        {
            entity.HasKey(e => e.ApprovalId).HasName("PK__Approval__328477F4F86DBABC");

            entity.ToTable("ApprovalQueue");

            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RequestDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Product).WithMany(p => p.ApprovalQueues)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ApprovalQ__Produ__3D5E1FD2");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CDC2B1FE4A");

            entity.Property(e => e.IsInApprovalQueue).HasDefaultValue(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PostedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
