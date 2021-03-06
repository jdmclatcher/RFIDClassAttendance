﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RFIDAttendance.Data;

namespace RFIDAttendance.Migrations
{
    [DbContext(typeof(StudentDbContext))]
    [Migration("20200208180710_Create-3")]
    partial class Create3
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("RFIDAttendance.Models.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AttendaceStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("InClass")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Period")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("StudentID")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("TimeLastCheckedIn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("TimeLastCheckedOut")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Student");
                });
#pragma warning restore 612, 618
        }
    }
}
