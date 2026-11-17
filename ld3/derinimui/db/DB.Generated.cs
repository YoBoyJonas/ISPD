using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Org.Ktu.T120B197.DBs.Entities;

namespace Org.Ktu.T120B197.DBs;

public partial class DB : DbContext
{
    private DB(DbContextOptions<DB> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Author { get; set; }

    public virtual DbSet<Book> Book { get; set; }

    public virtual DbSet<Customer> Customer { get; set; }

    public virtual DbSet<OrderItem> OrderItem { get; set; }

    public virtual DbSet<Orders> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasOne(d => d.Author).WithMany(p => p.Book)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_book_author");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasOne(d => d.Book).WithMany(p => p.OrderItem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_item_book");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_item_order");
        });

        modelBuilder.Entity<Orders>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orders_customer");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}



