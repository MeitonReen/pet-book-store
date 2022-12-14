// <auto-generated />
using System;
using BookStore.UserService.Data.BaseDatabase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookStore.UserService.Data.BaseDatabase.Migrations
{
    [DbContext(typeof(BaseDbContext))]
    partial class BaseDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BookStore.UserService.BL.ResourceEntities.Book", b =>
                {
                    b.Property<Guid>("BookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.HasKey("BookId");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("BookStore.UserService.BL.ResourceEntities.BookRating", b =>
                {
                    b.Property<Guid>("RatingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("BookId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateTimeSet")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ProfileUserId")
                        .HasColumnType("uuid");

                    b.Property<int>("Rating")
                        .HasColumnType("integer");

                    b.HasKey("RatingId");

                    b.HasIndex("BookId");

                    b.HasIndex("ProfileUserId");

                    b.ToTable("BookRatings");
                });

            modelBuilder.Entity("BookStore.UserService.BL.ResourceEntities.BookReview", b =>
                {
                    b.Property<Guid>("ReviewId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("BookId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateTimeSet")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ProfileUserId")
                        .HasColumnType("uuid");

                    b.Property<string>("Review")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ReviewId");

                    b.HasIndex("BookId");

                    b.HasIndex("ProfileUserId");

                    b.ToTable("BookReviews");
                });

            modelBuilder.Entity("BookStore.UserService.BL.ResourceEntities.Profile", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Patronymic")
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.InboxState", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("Consumed")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ConsumerId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("Delivered")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ExpirationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long?>("LastSequenceNumber")
                        .HasColumnType("bigint");

                    b.Property<Guid>("MessageId")
                        .HasColumnType("uuid");

                    b.Property<int>("ReceiveCount")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Received")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Delivered");

                    b.HasIndex("MessageId", "ConsumerId")
                        .IsUnique();

                    b.ToTable("InboxState");
                });

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.OutboxMessage", b =>
                {
                    b.Property<long>("SequenceNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("SequenceNumber"));

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<Guid?>("ConversationId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CorrelationId")
                        .HasColumnType("uuid");

                    b.Property<string>("DestinationAddress")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime?>("EnqueueTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ExpirationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FaultAddress")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Headers")
                        .HasColumnType("text");

                    b.Property<Guid?>("InboxConsumerId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("InboxMessageId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("InitiatorId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("MessageId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("OutboxId")
                        .HasColumnType("uuid");

                    b.Property<string>("Properties")
                        .HasColumnType("text");

                    b.Property<Guid?>("RequestId")
                        .HasColumnType("uuid");

                    b.Property<string>("ResponseAddress")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("SentTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SourceAddress")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("SequenceNumber");

                    b.HasIndex("EnqueueTime");

                    b.HasIndex("ExpirationTime");

                    b.HasIndex("OutboxId", "SequenceNumber")
                        .IsUnique();

                    b.HasIndex("InboxMessageId", "InboxConsumerId", "SequenceNumber")
                        .IsUnique();

                    b.ToTable("OutboxMessage");
                });

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.OutboxState", b =>
                {
                    b.Property<Guid>("OutboxId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("Delivered")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long?>("LastSequenceNumber")
                        .HasColumnType("bigint");

                    b.HasKey("OutboxId");

                    b.HasIndex("Created");

                    b.ToTable("OutboxState");
                });

            modelBuilder.Entity("BookStore.UserService.BL.ResourceEntities.BookRating", b =>
                {
                    b.HasOne("BookStore.UserService.BL.ResourceEntities.Book", "Book")
                        .WithMany("BookRatings")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("BookStore.UserService.BL.ResourceEntities.Profile", "Profile")
                        .WithMany("BookRatings")
                        .HasForeignKey("ProfileUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("BookStore.UserService.BL.ResourceEntities.BookReview", b =>
                {
                    b.HasOne("BookStore.UserService.BL.ResourceEntities.Book", "Book")
                        .WithMany("BookReviews")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("BookStore.UserService.BL.ResourceEntities.Profile", "Profile")
                        .WithMany("BookReviews")
                        .HasForeignKey("ProfileUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("BookStore.UserService.BL.ResourceEntities.Book", b =>
                {
                    b.Navigation("BookRatings");

                    b.Navigation("BookReviews");
                });

            modelBuilder.Entity("BookStore.UserService.BL.ResourceEntities.Profile", b =>
                {
                    b.Navigation("BookRatings");

                    b.Navigation("BookReviews");
                });
#pragma warning restore 612, 618
        }
    }
}
