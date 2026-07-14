using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travellio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHomePlaceName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "home_place_name",
                table: "trips",
                type: "character varying(60)",
                maxLength: 60,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "home_place_name",
                table: "trips");
        }
    }
}
