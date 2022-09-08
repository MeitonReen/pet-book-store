﻿// <auto-generated />
using System;
using BookStore.OrderService.Data.BaseDatabase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookStore.OrderService.Data.BaseDatabase.Migrations
{
    [DbContext(typeof(BaseDbContext))]
    [Migration("20220905022757_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BookStore.OrderService.BL.ResourceEntities.Book", b =>
                {
                    b.Property<Guid>("BookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<DateOnly>("PublicationDate")
                        .HasColumnType("date");

                    b.HasKey("BookId");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("BookStore.OrderService.BL.ResourceEntities.BookInCart", b =>
                {
                    b.Property<Guid>("BookInCartId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("BookId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("CartId")
                        .HasColumnType("uuid");

                    b.Property<int>("Count")
                        .HasColumnType("integer");

                    b.HasKey("BookInCartId");

                    b.HasIndex("BookId");

                    b.HasIndex("CartId");

                    b.ToTable("BooksInCarts");
                });

            modelBuilder.Entity("BookStore.OrderService.BL.ResourceEntities.BookInOrder", b =>
                {
                    b.Property<Guid>("BookInOrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("BookId")
                        .HasColumnType("uuid");

                    b.Property<int>("Count")
                        .HasColumnType("integer");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uuid");

                    b.HasKey("BookInOrderId");

                    b.HasIndex("BookId");

                    b.HasIndex("OrderId");

                    b.ToTable("BooksInOrders");
                });

            modelBuilder.Entity("BookStore.OrderService.BL.ResourceEntities.Cart", b =>
                {
                    b.Property<Guid>("CartId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CheckoutDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ProfileUserId")
                        .HasColumnType("uuid");

                    b.HasKey("CartId");

                    b.HasIndex("ProfileUserId");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("BookStore.OrderService.BL.ResourceEntities.Order", b =>
                {
                    b.Property<Guid>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ProfileUserId")
                        .HasColumnType("uuid");

                    b.HasKey("OrderId");

                    b.HasIndex("ProfileUserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("BookStore.OrderService.BL.ResourceEntities.Profile", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.HasKey("UserId");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("BookStore.OrderService.BL.ResourceEntities.BookInCart", b =>
                {
                    b.HasOne("BookStore.OrderService.BL.ResourceEntities.Book", "Book")
                        .WithMany("Carts")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookStore.OrderService.BL.ResourceEntities.Cart", "Cart")
                        .WithMany("Books")
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("Cart");
                });

            modelBuilder.Entity("BookStore.OrderService.BL.ResourceEntities.BookInOrder", b =>
                {
                    b.HasOne("BookStore.OrderService.BL.ResourceEntities.Book", "Book")
                        .WithMany("Orders")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookStore.OrderService.BL.ResourceEntities.Order", "Order")
                        .WithMany("Books")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("BookStore.OrderService.BL.ResourceEntities.Cart", b =>
                {
                    b.HasOne("BookStore.OrderService.BL.ResourceEntities.Profile", "Profile")
                        .WithMany("Carts")
                        .HasForeignKey("ProfileUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("BookStore.OrderService.BL.ResourceEntities.Order", b =>
                {
                    b.HasOne("BookStore.OrderService.BL.ResourceEntities.Profile", "Profile")
                        .WithMany("Orders")
                        .HasForeignKey("ProfileUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("BookStore.OrderService.BL.ResourceEntities.Book", b =>
                {
                    b.Navigation("Carts");

                    b.Navigation("Orders");
                });

            modelBuilder.Entity("BookStore.OrderService.BL.ResourceEntities.Cart", b =>
                {
                    b.Navigation("Books");
                });

            modelBuilder.Entity("BookStore.OrderService.BL.ResourceEntities.Order", b =>
                {
                    b.Navigation("Books");
                });

            modelBuilder.Entity("BookStore.OrderService.BL.ResourceEntities.Profile", b =>
                {
                    b.Navigation("Carts");

                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
