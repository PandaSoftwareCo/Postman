using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PostmanApi.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Contacts");

            migrationBuilder.CreateTable(
                name: "Contacts",
                schema: "Contacts",
                columns: table => new
                {
                    ContactId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAdult = table.Column<bool>(type: "bit", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<int>(type: "int", nullable: true),
                    ImageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Size = table.Column<long>(type: "bigint", nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImageWidth = table.Column<int>(type: "int", nullable: true),
                    ImageHeight = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ContactId", x => x.ContactId);
                });

            migrationBuilder.InsertData(
                schema: "Contacts",
                table: "Contacts",
                columns: new[] { "ContactId", "Age", "Birthday", "ContentType", "Email", "Gender", "Image", "ImageHeight", "ImageName", "ImageWidth", "IsAdult", "Name", "Phone", "Position", "Size" },
                values: new object[] { 1L, 40, new DateTime(1981, 5, 1, 0, 0, 0, 0, DateTimeKind.Local), null, "007@mi6.gov.uk", 0, null, null, null, null, true, "James Bond", "604-555-5555", 3, null });

            migrationBuilder.InsertData(
                schema: "Contacts",
                table: "Contacts",
                columns: new[] { "ContactId", "Age", "Birthday", "ContentType", "Email", "Gender", "Image", "ImageHeight", "ImageName", "ImageWidth", "IsAdult", "Name", "Phone", "Position", "Size" },
                values: new object[] { 2L, 30, new DateTime(1991, 5, 1, 0, 0, 0, 0, DateTimeKind.Local), null, "bourne@threadstone.com", 0, null, null, null, null, true, "Jason Bourne", "604-555-5555", 2, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contacts",
                schema: "Contacts");
        }
    }
}
