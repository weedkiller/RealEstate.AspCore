﻿// <auto-generated />
using System;
using GeoAPI.Geometries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RealEstate.Services.Database;

namespace RealEstate.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190412011333_SomeSwaps3")]
    partial class SomeSwaps3
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Applicant", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<string>("ContactId")
                        .IsRequired();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Description");

                    b.Property<string>("ItemRequestId");

                    b.Property<int>("Type");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ContactId");

                    b.HasIndex("ItemRequestId");

                    b.HasIndex("UserId");

                    b.ToTable("Applicant");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.ApplicantFeature", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApplicantId")
                        .IsRequired();

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("FeatureId")
                        .IsRequired();

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ApplicantId");

                    b.HasIndex("FeatureId");

                    b.ToTable("ApplicantFeature");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Beneficiary", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<int>("CommissionPercent");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("DealId")
                        .IsRequired();

                    b.Property<int>("TipPercent");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("DealId");

                    b.HasIndex("UserId");

                    b.ToTable("Beneficiary");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Category", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Contact", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<bool>("IsPrivate");

                    b.Property<string>("MobileNumber")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("PhoneNumber");

                    b.HasKey("Id");

                    b.ToTable("Contact");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Deal", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Description");

                    b.Property<string>("ItemRequestId");

                    b.HasKey("Id");

                    b.HasIndex("ItemRequestId")
                        .IsUnique()
                        .HasFilter("[ItemRequestId] IS NOT NULL");

                    b.ToTable("Deal");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.DealPayment", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<decimal>("CommissionPrice");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("DealId")
                        .IsRequired();

                    b.Property<DateTime>("PayDate");

                    b.Property<string>("Text");

                    b.Property<decimal>("TipPrice");

                    b.HasKey("Id");

                    b.HasIndex("DealId");

                    b.ToTable("DealPayment");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.District", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("District");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Facility", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Facility");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Feature", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("Feature");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.FixedSalary", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.Property<double>("Value");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("FixedSalary");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Item", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<string>("CategoryId")
                        .IsRequired();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Description");

                    b.Property<string>("PropertyId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("PropertyId");

                    b.ToTable("Item");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.ItemFeature", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("FeatureId")
                        .IsRequired();

                    b.Property<string>("ItemId")
                        .IsRequired();

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("FeatureId");

                    b.HasIndex("ItemId");

                    b.ToTable("ItemFeature");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.ItemRequest", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Description");

                    b.Property<bool>("IsReject");

                    b.Property<string>("ItemId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.ToTable("ItemRequest");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Ownership", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<string>("ContactId")
                        .IsRequired();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Description");

                    b.Property<int>("Dong");

                    b.Property<string>("PropertyOwnershipId");

                    b.HasKey("Id");

                    b.HasIndex("ContactId");

                    b.HasIndex("PropertyOwnershipId");

                    b.ToTable("Ownership");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Payment", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Text");

                    b.Property<int>("Type");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.Property<double>("Value");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Payment");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Permission", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Key");

                    b.Property<int>("Type");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Permission");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Picture", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("DealId");

                    b.Property<string>("DealPaymentId");

                    b.Property<string>("File")
                        .IsRequired();

                    b.Property<string>("PaymentId");

                    b.Property<string>("PropertyId");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("DealId");

                    b.HasIndex("DealPaymentId");

                    b.HasIndex("PaymentId");

                    b.HasIndex("PropertyId");

                    b.ToTable("Picture");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Property", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Alley");

                    b.Property<string>("Audit");

                    b.Property<string>("BuildingName");

                    b.Property<string>("CategoryId");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Description");

                    b.Property<string>("DistrictId")
                        .IsRequired();

                    b.Property<int>("Flat");

                    b.Property<int>("Floor");

                    b.Property<IPoint>("Geolocation");

                    b.Property<string>("Number");

                    b.Property<string>("Street")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("DistrictId");

                    b.ToTable("Property");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.PropertyFacility", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("FacilityId")
                        .IsRequired();

                    b.Property<string>("PropertyId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("FacilityId");

                    b.HasIndex("PropertyId");

                    b.ToTable("PropertyFacility");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.PropertyFeature", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("FeatureId")
                        .IsRequired();

                    b.Property<string>("PropertyId")
                        .IsRequired();

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("FeatureId");

                    b.HasIndex("PropertyId");

                    b.ToTable("PropertyFeature");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.PropertyOwnership", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("PropertyId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("PropertyId");

                    b.ToTable("PropertyOwnership");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Reminder", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AlarmTime");

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Text");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Reminder");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Sms", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<string>("ContactId");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<int>("Provider");

                    b.Property<string>("Receiver");

                    b.Property<string>("ReferenceId");

                    b.Property<string>("Sender");

                    b.Property<string>("SmsTemplateId");

                    b.Property<string>("StatusJson");

                    b.Property<string>("Text");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ContactId");

                    b.HasIndex("SmsTemplateId");

                    b.HasIndex("UserId");

                    b.ToTable("Sms");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.SmsTemplate", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.ToTable("SmsTemplate");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<string>("Audit");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("Mobile")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<string>("Phone")
                        .IsRequired();

                    b.Property<int>("Role");

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Mobile")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.UserItemCategory", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<string>("CategoryId")
                        .IsRequired();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("UserId");

                    b.ToTable("UserItemCategory");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.UserPropertyCategory", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audit");

                    b.Property<string>("CategoryId")
                        .IsRequired();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("UserId");

                    b.ToTable("UserPropertyCategory");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Applicant", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.Contact", "Contact")
                        .WithMany("Applicants")
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Services.Database.Tables.ItemRequest", "ItemRequest")
                        .WithMany("Applicants")
                        .HasForeignKey("ItemRequestId");

                    b.HasOne("RealEstate.Services.Database.Tables.User", "User")
                        .WithMany("Applicants")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.ApplicantFeature", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.Applicant", "Applicant")
                        .WithMany("ApplicantFeatures")
                        .HasForeignKey("ApplicantId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Services.Database.Tables.Feature", "Feature")
                        .WithMany("ApplicantFeatures")
                        .HasForeignKey("FeatureId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Beneficiary", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.Deal", "Deal")
                        .WithMany("Beneficiaries")
                        .HasForeignKey("DealId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Services.Database.Tables.User", "User")
                        .WithMany("Beneficiaries")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Deal", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.ItemRequest", "ItemRequest")
                        .WithOne("Deal")
                        .HasForeignKey("RealEstate.Services.Database.Tables.Deal", "ItemRequestId");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.DealPayment", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.Deal", "Deal")
                        .WithMany("DealPayments")
                        .HasForeignKey("DealId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.FixedSalary", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.User", "User")
                        .WithMany("FixedSalaries")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Item", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.Category", "Category")
                        .WithMany("Items")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Services.Database.Tables.Property", "Property")
                        .WithMany("Items")
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.ItemFeature", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.Feature", "Feature")
                        .WithMany("ItemFeatures")
                        .HasForeignKey("FeatureId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Services.Database.Tables.Item", "Item")
                        .WithMany("ItemFeatures")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.ItemRequest", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.Item", "Item")
                        .WithMany("ItemRequests")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Ownership", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.Contact", "Contact")
                        .WithMany("Ownerships")
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Services.Database.Tables.PropertyOwnership", "PropertyOwnership")
                        .WithMany("Ownerships")
                        .HasForeignKey("PropertyOwnershipId");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Payment", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.User", "User")
                        .WithMany("Payments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Permission", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.User", "User")
                        .WithMany("Permissions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Picture", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.Deal", "Deal")
                        .WithMany("Pictures")
                        .HasForeignKey("DealId");

                    b.HasOne("RealEstate.Services.Database.Tables.DealPayment", "DealPayment")
                        .WithMany("Pictures")
                        .HasForeignKey("DealPaymentId");

                    b.HasOne("RealEstate.Services.Database.Tables.Payment", "Payment")
                        .WithMany("Pictures")
                        .HasForeignKey("PaymentId");

                    b.HasOne("RealEstate.Services.Database.Tables.Property", "Property")
                        .WithMany("Pictures")
                        .HasForeignKey("PropertyId");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Property", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.Category", "Category")
                        .WithMany("Properties")
                        .HasForeignKey("CategoryId");

                    b.HasOne("RealEstate.Services.Database.Tables.District", "District")
                        .WithMany("Properties")
                        .HasForeignKey("DistrictId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.PropertyFacility", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.Facility", "Facility")
                        .WithMany("PropertyFacilities")
                        .HasForeignKey("FacilityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Services.Database.Tables.Property", "Property")
                        .WithMany("PropertyFacilities")
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.PropertyFeature", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.Feature", "Feature")
                        .WithMany("PropertyFeatures")
                        .HasForeignKey("FeatureId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Services.Database.Tables.Property", "Property")
                        .WithMany("PropertyFeatures")
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.PropertyOwnership", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.Property", "Property")
                        .WithMany("PropertyOwnerships")
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Reminder", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.User", "User")
                        .WithMany("Reminders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.Sms", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.Contact", "Contact")
                        .WithMany("Smses")
                        .HasForeignKey("ContactId");

                    b.HasOne("RealEstate.Services.Database.Tables.SmsTemplate", "SmsTemplate")
                        .WithMany("Smses")
                        .HasForeignKey("SmsTemplateId");

                    b.HasOne("RealEstate.Services.Database.Tables.User", "User")
                        .WithMany("Smses")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.UserItemCategory", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.Category", "Category")
                        .WithMany("UserItemCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Services.Database.Tables.User", "User")
                        .WithMany("UserItemCategories")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Services.Database.Tables.UserPropertyCategory", b =>
                {
                    b.HasOne("RealEstate.Services.Database.Tables.Category", "Category")
                        .WithMany("UserPropertyCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Services.Database.Tables.User", "User")
                        .WithMany("UserPropertyCategories")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
