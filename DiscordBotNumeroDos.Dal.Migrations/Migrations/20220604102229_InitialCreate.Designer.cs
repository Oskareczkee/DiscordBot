﻿// <auto-generated />
using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiscordBotNumeroDos.Dal.Migrations.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20220604102229_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("DAL.Models.Items.Item", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("Agility")
                        .HasColumnType("int");

                    b.Property<int>("Armor")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Endurance")
                        .HasColumnType("int");

                    b.Property<int>("Intelligence")
                        .HasColumnType("int");

                    b.Property<int>("Luck")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int>("Strength")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("DAL.Models.Profiles.EquipmentItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("ItemID")
                        .HasColumnType("int");

                    b.Property<int>("ProfileID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("ItemID");

                    b.HasIndex("ProfileID");

                    b.ToTable("EquipmentItems");
                });

            modelBuilder.Entity("DAL.Models.Profiles.Profile", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("Agility")
                        .HasColumnType("int");

                    b.Property<int>("Armor")
                        .HasColumnType("int");

                    b.Property<decimal>("DiscordID")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("Endurance")
                        .HasColumnType("int");

                    b.Property<double>("Gold")
                        .HasColumnType("float");

                    b.Property<decimal>("GuildID")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("Intelligence")
                        .HasColumnType("int");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<int>("Luck")
                        .HasColumnType("int");

                    b.Property<int>("NextLevel")
                        .HasColumnType("int");

                    b.Property<int>("Strength")
                        .HasColumnType("int");

                    b.Property<int>("XP")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("DAL.Models.Profiles.ProfileItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("ItemID")
                        .HasColumnType("int");

                    b.Property<int>("ProfileID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("ItemID");

                    b.HasIndex("ProfileID");

                    b.ToTable("ProfileItems");
                });

            modelBuilder.Entity("DAL.Models.Profiles.EquipmentItem", b =>
                {
                    b.HasOne("DAL.Models.Items.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemID");

                    b.HasOne("DAL.Models.Profiles.Profile", "Profile")
                        .WithMany("Equipment")
                        .HasForeignKey("ProfileID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("DAL.Models.Profiles.ProfileItem", b =>
                {
                    b.HasOne("DAL.Models.Items.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemID");

                    b.HasOne("DAL.Models.Profiles.Profile", "Profile")
                        .WithMany("Items")
                        .HasForeignKey("ProfileID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("DAL.Models.Profiles.Profile", b =>
                {
                    b.Navigation("Equipment");

                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
