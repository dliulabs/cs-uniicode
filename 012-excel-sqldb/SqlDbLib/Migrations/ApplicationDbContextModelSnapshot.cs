﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SqlDbLib;

namespace SqlDbLib.Migrations {
    [DbContext (typeof (ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot {
        protected override void BuildModel (ModelBuilder modelBuilder) {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation ("ProductVersion", "6.0.8")
                .HasAnnotation ("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns (modelBuilder, 1L, 1);

            modelBuilder.Entity ("SqlDbLib.Entities.TrialBalance", b => {
                b.Property<long> ("Id")
                    .ValueGeneratedOnAdd ()
                    .HasColumnType ("bigint");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn (b.Property<long> ("Id"), 1L, 1);

                b.Property<decimal?> ("BalanceCarryover")
                    .HasPrecision (18, 2)
                    .HasColumnType ("decimal(18,2)");

                b.Property<string> ("Code")
                    .IsRequired ()
                    .HasMaxLength (100)
                    .IsUnicode (false)
                    .HasColumnType ("varchar(100)");

                b.Property<decimal?> ("CumulativeBalance")
                    .HasPrecision (18, 2)
                    .HasColumnType ("decimal(18,2)");

                b.Property<string> ("Currency")
                    .IsRequired ()
                    .HasMaxLength (100)
                    .IsUnicode (false)
                    .HasColumnType ("varchar(100)");

                b.Property<string> ("Description")
                    .IsRequired ()
                    .HasMaxLength (100)
                    .IsUnicode (true)
                    .HasColumnType ("varchar(100)")
                    .UseCollation ("Latin1_General_100_BIN2_UTF8");

                b.Property<string> ("GLAccount")
                    .IsRequired ()
                    .HasMaxLength (100)
                    .IsUnicode (false)
                    .HasColumnType ("varchar(100)");

                b.Property<decimal?> ("PeriodCreditReporting")
                    .HasPrecision (18, 2)
                    .HasColumnType ("decimal(18,2)");

                b.Property<decimal?> ("PeriodDebtReporting")
                    .HasPrecision (18, 2)
                    .HasColumnType ("decimal(18,2)");

                b.HasKey ("Id");

                b.ToTable ("TrialBalance");
            });
#pragma warning restore 612, 618
        }
    }
}