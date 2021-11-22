using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostmanApi.Models;

namespace PostmanApi.Data
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.ToTable("Contacts", "Contacts");

            builder.HasKey(e => e.ContactId).HasName("ContactId");

            builder.Property(e => e.Name).HasColumnName("Name").IsRequired().HasMaxLength(100);

            builder.Property(e => e.Email).HasColumnName("Email").HasMaxLength(100);

            builder.Property(e => e.Phone).HasColumnName("Phone").HasMaxLength(20);

            //builder.Property(t => t.OrderDate)
            //    .IsRequired()
            //    .HasColumnType("Date")
            //    .HasDefaultValueSql("GetDate()");

            builder.HasData(
                new Contact
                {
                    ContactId = 1,
                    Name = "James Bond",
                    Phone = "604-555-5555",
                    Email = "007@mi6.gov.uk",
                    Age = 40,
                    Birthday = DateTime.Today.AddYears(-40),
                    IsAdult = true,
                    Gender = Gender.Male,
                    Position = Position.Spy
                },
                new Contact
                {
                    ContactId = 2,
                    Name = "Jason Bourne",
                    Phone = "604-555-5555",
                    Email = "bourne@threadstone.com",
                    Age = 30,
                    Birthday = DateTime.Today.AddYears(-30),
                    IsAdult = true,
                    Gender = Gender.Male,
                    Position = Position.Soldier
                });
        }
    }
}
