using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCoinOS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Address_RemoveProvince : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Province",
                table: "Addresses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Addresses",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);
        }
    }
}
