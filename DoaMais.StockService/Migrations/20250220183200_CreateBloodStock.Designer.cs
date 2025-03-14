﻿// <auto-generated />
using System;
using DoaMais.StockService.Model.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DoaMais.StockService.Migrations
{
    [DbContext(typeof(SQLServerContext))]
    [Migration("20250220183200_CreateBloodStock")]
    partial class CreateBloodStock
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DoaMais.StockService.Model.BloodStock", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id");

                    b.Property<string>("BloodType")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("BloodType");

                    b.Property<int>("QuantityML")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0)
                        .HasColumnName("QuantityML");

                    b.Property<string>("RHFactor")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("RHFactor");

                    b.HasKey("Id");

                    b.HasIndex("BloodType", "RHFactor")
                        .IsUnique();

                    b.ToTable("BloodStocks");
                });
#pragma warning restore 612, 618
        }
    }
}
