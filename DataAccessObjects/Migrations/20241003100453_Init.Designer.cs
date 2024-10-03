﻿// <auto-generated />
using System;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataAccessObjects.Migrations
{
    [DbContext(typeof(Fall2024DbContext))]
    [Migration("20241003100453_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BussinessObjects.AirPlane", b =>
                {
                    b.Property<int>("PlaneId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PlaneId"));

                    b.Property<int>("NormalSeatNumber")
                        .HasColumnType("int");

                    b.Property<string>("PlaneName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VipSeatNumber")
                        .HasColumnType("int");

                    b.HasKey("PlaneId");

                    b.ToTable("AirPlanes");
                });

            modelBuilder.Entity("BussinessObjects.Booking", b =>
                {
                    b.Property<int>("BookingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BookingId"));

                    b.Property<int>("AdultNum")
                        .HasColumnType("int");

                    b.Property<int>("BabyNum")
                        .HasColumnType("int");

                    b.Property<DateTime>("BookingDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ChildNum")
                        .HasColumnType("int");

                    b.Property<int>("FlightId")
                        .HasColumnType("int");

                    b.Property<string>("PaymentStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ReturnFlightId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("BookingId");

                    b.HasIndex("FlightId");

                    b.HasIndex("ReturnFlightId");

                    b.HasIndex("UserId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("BussinessObjects.Flight", b =>
                {
                    b.Property<int>("FlightId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FlightId"));

                    b.Property<DateTime>("ArrivalDateTime")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("BasePrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("DepartureDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("DestinationID")
                        .HasColumnType("int");

                    b.Property<string>("FlightNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OriginID")
                        .HasColumnType("int");

                    b.Property<int>("PilotId")
                        .HasColumnType("int");

                    b.Property<int>("PlaneId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FlightId");

                    b.HasIndex("DestinationID");

                    b.HasIndex("OriginID");

                    b.HasIndex("PilotId");

                    b.HasIndex("PlaneId");

                    b.ToTable("Flights");
                });

            modelBuilder.Entity("BussinessObjects.Location", b =>
                {
                    b.Property<int>("LocationID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LocationID"));

                    b.Property<string>("LocationName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LocationID");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("BussinessObjects.Payment", b =>
                {
                    b.Property<int>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PaymentId"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("BookingId")
                        .HasColumnType("int");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("PaymentId");

                    b.HasIndex("BookingId")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("BussinessObjects.Pilot", b =>
                {
                    b.Property<int>("PilotId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PilotId"));

                    b.Property<string>("PilotName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PilotId");

                    b.ToTable("Pilots");
                });

            modelBuilder.Entity("BussinessObjects.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoleId"));

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RoleId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("BussinessObjects.Ticket", b =>
                {
                    b.Property<int>("TicketId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TicketId"));

                    b.Property<decimal>("Baggage")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("BookingId")
                        .HasColumnType("int");

                    b.Property<decimal>("Carryluggage")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ClassType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("IssuedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SeatNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TicketType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TicketId");

                    b.HasIndex("BookingId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("BussinessObjects.Tier", b =>
                {
                    b.Property<int>("TierId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TierId"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Discount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("PriorityLevel")
                        .HasColumnType("int");

                    b.Property<string>("TierName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TierId");

                    b.ToTable("Tiers");
                });

            modelBuilder.Entity("BussinessObjects.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("CCCD")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DoB")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TierId")
                        .HasColumnType("int");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("TierId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("BussinessObjects.Booking", b =>
                {
                    b.HasOne("BussinessObjects.Flight", "Flight")
                        .WithMany()
                        .HasForeignKey("FlightId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BussinessObjects.Flight", "ReturnFlight")
                        .WithMany()
                        .HasForeignKey("ReturnFlightId");

                    b.HasOne("BussinessObjects.User", "User")
                        .WithMany("Bookings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Flight");

                    b.Navigation("ReturnFlight");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BussinessObjects.Flight", b =>
                {
                    b.HasOne("BussinessObjects.Location", "Destination")
                        .WithMany()
                        .HasForeignKey("DestinationID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BussinessObjects.Location", "Origin")
                        .WithMany()
                        .HasForeignKey("OriginID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BussinessObjects.Pilot", "Pilot")
                        .WithMany()
                        .HasForeignKey("PilotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BussinessObjects.AirPlane", "Plane")
                        .WithMany("Flights")
                        .HasForeignKey("PlaneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Destination");

                    b.Navigation("Origin");

                    b.Navigation("Pilot");

                    b.Navigation("Plane");
                });

            modelBuilder.Entity("BussinessObjects.Payment", b =>
                {
                    b.HasOne("BussinessObjects.Booking", "Booking")
                        .WithOne("Payment")
                        .HasForeignKey("BussinessObjects.Payment", "BookingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BussinessObjects.User", "User")
                        .WithMany("Payments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Booking");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BussinessObjects.Ticket", b =>
                {
                    b.HasOne("BussinessObjects.Booking", "Booking")
                        .WithMany("Tickets")
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Booking");
                });

            modelBuilder.Entity("BussinessObjects.User", b =>
                {
                    b.HasOne("BussinessObjects.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BussinessObjects.Tier", "Tier")
                        .WithMany("Users")
                        .HasForeignKey("TierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("Tier");
                });

            modelBuilder.Entity("BussinessObjects.AirPlane", b =>
                {
                    b.Navigation("Flights");
                });

            modelBuilder.Entity("BussinessObjects.Booking", b =>
                {
                    b.Navigation("Payment")
                        .IsRequired();

                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("BussinessObjects.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("BussinessObjects.Tier", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("BussinessObjects.User", b =>
                {
                    b.Navigation("Bookings");

                    b.Navigation("Payments");
                });
#pragma warning restore 612, 618
        }
    }
}
