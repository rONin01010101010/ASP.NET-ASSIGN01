﻿// <auto-generated />
using System;
using COMP2139_assign01.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace COMP2139_assign01.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250216002625_AddProductDescriptionAndSKU")]
    partial class AddProductDescriptionAndSKU
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("COMP2139_assign01.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CategoryId"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("CategoryId");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            CategoryId = 1,
                            Description = "Electronic devices and accessories",
                            Name = "Electronics"
                        },
                        new
                        {
                            CategoryId = 2,
                            Description = "Household items and furniture",
                            Name = "Household"
                        });
                });

            modelBuilder.Entity("COMP2139_assign01.Models.Inventory", b =>
                {
                    b.Property<int>("InventoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("InventoryId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<int>("TotalStock")
                        .HasColumnType("integer");

                    b.Property<string>("category")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("InventoryId");

                    b.ToTable("Inventory");

                    b.HasData(
                        new
                        {
                            InventoryId = 1,
                            Name = "Carpet",
                            Price = 50m,
                            Quantity = 5,
                            TotalStock = 0,
                            category = "Household"
                        },
                        new
                        {
                            InventoryId = 2,
                            Name = "Laptop",
                            Price = 500m,
                            Quantity = 7,
                            TotalStock = 0,
                            category = "Electronics"
                        });
                });

            modelBuilder.Entity("COMP2139_assign01.Models.InventoryTask", b =>
                {
                    b.Property<int>("InventoryTaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("InventoryTaskId"));

                    b.Property<int>("InventoryId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<string>("category")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("InventoryTaskId");

                    b.HasIndex("InventoryId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("COMP2139_assign01.Models.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("OrderId"));

                    b.Property<string>("GuestEmail")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("GuestName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("GuestPhone")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("TrackingNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.HasKey("OrderId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("COMP2139_assign01.Models.OrderItem", b =>
                {
                    b.Property<int>("OrderItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("OrderItemId"));

                    b.Property<int>("OrderId")
                        .HasColumnType("integer");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("OrderItemId");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("COMP2139_assign01.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ProductId"));

                    b.Property<int?>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("LowStockThreshold")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<string>("SKU")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("ProductId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            ProductId = 1,
                            CategoryId = 1,
                            Description = "High-performance laptop with latest specifications",
                            ImageUrl = "/images/laptop.jpg",
                            LowStockThreshold = 5,
                            Name = "Laptop",
                            Price = 500m,
                            Quantity = 7,
                            SKU = "ELEC-LAP-001"
                        },
                        new
                        {
                            ProductId = 2,
                            CategoryId = 2,
                            Description = "Luxurious carpet for your living room",
                            ImageUrl = "/images/carpet.jpg",
                            LowStockThreshold = 3,
                            Name = "Carpet",
                            Price = 50m,
                            Quantity = 5,
                            SKU = "HOME-CAR-001"
                        });
                });

            modelBuilder.Entity("COMP2139_assign01.Models.InventoryTask", b =>
                {
                    b.HasOne("COMP2139_assign01.Models.Inventory", "Inventory")
                        .WithMany("Tasks")
                        .HasForeignKey("InventoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Inventory");
                });

            modelBuilder.Entity("COMP2139_assign01.Models.OrderItem", b =>
                {
                    b.HasOne("COMP2139_assign01.Models.Order", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("COMP2139_assign01.Models.Product", "Product")
                        .WithMany("OrderItems")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("COMP2139_assign01.Models.Product", b =>
                {
                    b.HasOne("COMP2139_assign01.Models.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("COMP2139_assign01.Models.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("COMP2139_assign01.Models.Inventory", b =>
                {
                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("COMP2139_assign01.Models.Order", b =>
                {
                    b.Navigation("OrderItems");
                });

            modelBuilder.Entity("COMP2139_assign01.Models.Product", b =>
                {
                    b.Navigation("OrderItems");
                });
#pragma warning restore 612, 618
        }
    }
}
