﻿// <auto-generated />
using System;
using ChargingStation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ChargingStation.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240504090007_FixTimeZOnes")]
    partial class FixTimeZOnes
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ChargingStation.Domain.Entities.ChargePoint", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ChargeBoxSerialNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ChargePointModel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ChargePointSerialNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ChargePointVendor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DepotId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DiagnosticsTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FirmwareUpdateTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirmwareVersion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Iccid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Imsi")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastHeartbeat")
                        .HasColumnType("datetime2");

                    b.Property<string>("MeterSerialNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MeterType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OcppProtocol")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegistrationStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DepotId");

                    b.ToTable("ChargePoints");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.ChargePointEnergyConsumptionSettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("ChargePointEnergyLimit")
                        .HasColumnType("float");

                    b.Property<Guid>("ChargePointId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DepotEnergyConsumptionSettingsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ChargePointId");

                    b.HasIndex("DepotEnergyConsumptionSettingsId");

                    b.ToTable("ChargePointEnergyConsumptionSettings");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.ChargingProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ChargingProfileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ChargingProfileId"));

                    b.Property<int>("ChargingProfileKind")
                        .HasColumnType("int");

                    b.Property<int>("ChargingProfilePurpose")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<decimal>("MinChargingRate")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("RecurrencyKind")
                        .HasColumnType("int");

                    b.Property<int>("SchedulingUnit")
                        .HasColumnType("int");

                    b.Property<int>("StackLevel")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartSchedule")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ValidFrom")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ValidTo")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("ChargingProfile");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.ChargingSchedulePeriod", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChargingProfileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Limit")
                        .HasColumnType("float");

                    b.Property<int>("NumberPhases")
                        .HasColumnType("int");

                    b.Property<int>("StartPeriod")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ChargingProfileId");

                    b.ToTable("ChargingSchedulePeriod");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.Connector", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChargePointId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ConnectorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ChargePointId", "ConnectorId")
                        .IsUnique();

                    b.ToTable("Connectors");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.ConnectorChargingProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChargingProfileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ConnectorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ChargingProfileId");

                    b.HasIndex("ConnectorId");

                    b.ToTable("ConnectorChargingProfile");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.ConnectorMeterValue", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ConnectorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Format")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Measurand")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("MeterValueTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("Phase")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TransactionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Unit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ConnectorId");

                    b.HasIndex("TransactionId");

                    b.ToTable("ConnectorMeterValue");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.ConnectorStatus", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ConnectorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CurrentStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ErrorCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Info")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("StatusUpdatedTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("VendorErrorCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VendorId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ConnectorId");

                    b.ToTable("ConnectorStatus");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.Depot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Building")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TimeZoneId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("TimeZoneId");

                    b.ToTable("Depots");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.DepotEnergyConsumptionSettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<double>("DepotEnergyLimit")
                        .HasColumnType("float");

                    b.Property<Guid>("DepotId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ValidFrom")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ValidTo")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DepotId");

                    b.ToTable("DepotEnergyConsumptionSettings");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.EnergyConsumptionIntervalSettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DepotEnergyConsumptionSettingsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<double>("EnergyLimit")
                        .HasColumnType("float");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DepotEnergyConsumptionSettingsId");

                    b.ToTable("EnergyConsumptionIntervalSettings");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.OcppTag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool?>("Blocked")
                        .HasColumnType("bit");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ParentTagId")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("TagId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("TagId")
                        .IsUnique();

                    b.ToTable("OcppTags");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.OcppTransaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ChargePointId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ConnectorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<double>("MeterStart")
                        .HasColumnType("float");

                    b.Property<double?>("MeterStop")
                        .HasColumnType("float");

                    b.Property<string>("StartResult")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("StartTagId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("StopReason")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("StopTagId")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("StopTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TransactionId"));

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ChargePointId");

                    b.HasIndex("ConnectorId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.Reservation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CancellationRequestId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ChargePointId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ConnectorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ExpiryDateTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsCancelled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ReservationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReservationId"));

                    b.Property<string>("ReservationRequestId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("StartDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TagId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("TransactionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ChargePointId");

                    b.HasIndex("ConnectorId");

                    b.HasIndex("ReservationRequestId")
                        .IsUnique();

                    b.HasIndex("TagId");

                    b.HasIndex("TransactionId")
                        .IsUnique()
                        .HasFilter("[TransactionId] IS NOT NULL");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.TimeZone", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BaseUtcOffset")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("IanaId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("WindowsId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("Id");

                    b.ToTable("TimeZones");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.ChargePoint", b =>
                {
                    b.HasOne("ChargingStation.Domain.Entities.Depot", "Depot")
                        .WithMany("ChargePoints")
                        .HasForeignKey("DepotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Depot");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.ChargePointEnergyConsumptionSettings", b =>
                {
                    b.HasOne("ChargingStation.Domain.Entities.ChargePoint", "ChargePoint")
                        .WithMany("EnergyConsumptionSettings")
                        .HasForeignKey("ChargePointId")
                        .IsRequired()
                        .HasConstraintName("FK_ChargePointEnergyConsumptionSettings_ChargePoint");

                    b.HasOne("ChargingStation.Domain.Entities.DepotEnergyConsumptionSettings", "DepotEnergyConsumptionSettings")
                        .WithMany("ChargePointsLimits")
                        .HasForeignKey("DepotEnergyConsumptionSettingsId")
                        .IsRequired()
                        .HasConstraintName("FK_ChargePointsLimits_DepotEnergyConsumptionSettings");

                    b.Navigation("ChargePoint");

                    b.Navigation("DepotEnergyConsumptionSettings");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.ChargingSchedulePeriod", b =>
                {
                    b.HasOne("ChargingStation.Domain.Entities.ChargingProfile", "ChargingProfile")
                        .WithMany("ChargingSchedulePeriods")
                        .HasForeignKey("ChargingProfileId")
                        .IsRequired()
                        .HasConstraintName("FK_ChargingSchedulePeriods_ChargingProfile");

                    b.Navigation("ChargingProfile");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.Connector", b =>
                {
                    b.HasOne("ChargingStation.Domain.Entities.ChargePoint", "ChargePoint")
                        .WithMany("Connectors")
                        .HasForeignKey("ChargePointId")
                        .IsRequired()
                        .HasConstraintName("FK_Connectors_ChargePoint");

                    b.Navigation("ChargePoint");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.ConnectorChargingProfile", b =>
                {
                    b.HasOne("ChargingStation.Domain.Entities.ChargingProfile", "ChargingProfile")
                        .WithMany("ConnectorChargingProfiles")
                        .HasForeignKey("ChargingProfileId")
                        .IsRequired()
                        .HasConstraintName("FK_ConnectorChargingProfiles_ChargingProfile");

                    b.HasOne("ChargingStation.Domain.Entities.Connector", "Connector")
                        .WithMany("ConnectorChargingProfiles")
                        .HasForeignKey("ConnectorId")
                        .IsRequired()
                        .HasConstraintName("FK_ConnectorChargingProfiles_Connector");

                    b.Navigation("ChargingProfile");

                    b.Navigation("Connector");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.ConnectorMeterValue", b =>
                {
                    b.HasOne("ChargingStation.Domain.Entities.Connector", "Connector")
                        .WithMany("ConnectorMeterValues")
                        .HasForeignKey("ConnectorId")
                        .IsRequired()
                        .HasConstraintName("FK_ConnectorMeterValues_Connector");

                    b.HasOne("ChargingStation.Domain.Entities.OcppTransaction", "Transaction")
                        .WithMany("ConnectorMeterValues")
                        .HasForeignKey("TransactionId")
                        .IsRequired()
                        .HasConstraintName("FK_ConnectorMeterValues_Transaction");

                    b.Navigation("Connector");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.ConnectorStatus", b =>
                {
                    b.HasOne("ChargingStation.Domain.Entities.Connector", "Connector")
                        .WithMany("ConnectorStatuses")
                        .HasForeignKey("ConnectorId")
                        .IsRequired()
                        .HasConstraintName("FK_ConnectorStatuses_Connector");

                    b.Navigation("Connector");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.Depot", b =>
                {
                    b.HasOne("ChargingStation.Domain.Entities.TimeZone", "TimeZone")
                        .WithMany("Depots")
                        .HasForeignKey("TimeZoneId")
                        .IsRequired()
                        .HasConstraintName("FK_Depot_TimeZone");

                    b.Navigation("TimeZone");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.DepotEnergyConsumptionSettings", b =>
                {
                    b.HasOne("ChargingStation.Domain.Entities.Depot", "Depot")
                        .WithMany("EnergyConsumptionSettings")
                        .HasForeignKey("DepotId")
                        .IsRequired()
                        .HasConstraintName("FK_DepotEnergyConsumptionSettings_Depot");

                    b.Navigation("Depot");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.EnergyConsumptionIntervalSettings", b =>
                {
                    b.HasOne("ChargingStation.Domain.Entities.DepotEnergyConsumptionSettings", "DepotEnergyConsumptionSettings")
                        .WithMany("Intervals")
                        .HasForeignKey("DepotEnergyConsumptionSettingsId")
                        .IsRequired()
                        .HasConstraintName("FK_Intervals_DepotEnergyConsumptionSettings");

                    b.Navigation("DepotEnergyConsumptionSettings");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.OcppTransaction", b =>
                {
                    b.HasOne("ChargingStation.Domain.Entities.ChargePoint", null)
                        .WithMany("Transactions")
                        .HasForeignKey("ChargePointId");

                    b.HasOne("ChargingStation.Domain.Entities.Connector", "Connector")
                        .WithMany()
                        .HasForeignKey("ConnectorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Connector");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.Reservation", b =>
                {
                    b.HasOne("ChargingStation.Domain.Entities.ChargePoint", "ChargePoint")
                        .WithMany("Reservations")
                        .HasForeignKey("ChargePointId")
                        .IsRequired()
                        .HasConstraintName("FK_Reservations_ChargePoint");

                    b.HasOne("ChargingStation.Domain.Entities.Connector", "Connector")
                        .WithMany("Reservations")
                        .HasForeignKey("ConnectorId");

                    b.HasOne("ChargingStation.Domain.Entities.OcppTag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ChargingStation.Domain.Entities.OcppTransaction", "Transaction")
                        .WithOne("Reservation")
                        .HasForeignKey("ChargingStation.Domain.Entities.Reservation", "TransactionId");

                    b.Navigation("ChargePoint");

                    b.Navigation("Connector");

                    b.Navigation("Tag");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.ChargePoint", b =>
                {
                    b.Navigation("Connectors");

                    b.Navigation("EnergyConsumptionSettings");

                    b.Navigation("Reservations");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.ChargingProfile", b =>
                {
                    b.Navigation("ChargingSchedulePeriods");

                    b.Navigation("ConnectorChargingProfiles");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.Connector", b =>
                {
                    b.Navigation("ConnectorChargingProfiles");

                    b.Navigation("ConnectorMeterValues");

                    b.Navigation("ConnectorStatuses");

                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.Depot", b =>
                {
                    b.Navigation("ChargePoints");

                    b.Navigation("EnergyConsumptionSettings");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.DepotEnergyConsumptionSettings", b =>
                {
                    b.Navigation("ChargePointsLimits");

                    b.Navigation("Intervals");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.OcppTransaction", b =>
                {
                    b.Navigation("ConnectorMeterValues");

                    b.Navigation("Reservation");
                });

            modelBuilder.Entity("ChargingStation.Domain.Entities.TimeZone", b =>
                {
                    b.Navigation("Depots");
                });
#pragma warning restore 612, 618
        }
    }
}
