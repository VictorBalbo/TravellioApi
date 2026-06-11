using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravellioApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trips",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    home_place_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trips", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "destinations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    place_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    notes = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_destinations", x => x.id);
                    table.ForeignKey(
                        name: "fk_destinations_trips_trip_id",
                        column: x => x.trip_id,
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "accommodations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    place_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    check_in = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    check_out = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    image_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    website = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    notes = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    price_value = table.Column<decimal>(type: "numeric(9,2)", precision: 9, scale: 2, nullable: true),
                    price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    destination_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accommodations", x => x.id);
                    table.ForeignKey(
                        name: "fk_accommodations_destinations_destination_id",
                        column: x => x.destination_id,
                        principalTable: "destinations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "activities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    place_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    scheduled_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ticket_required = table.Column<bool>(type: "boolean", nullable: true),
                    ticket_purchased = table.Column<bool>(type: "boolean", nullable: true),
                    price_value = table.Column<decimal>(type: "numeric(9,2)", precision: 9, scale: 2, nullable: true),
                    price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    website = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    notes = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    destination_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_activities", x => x.id);
                    table.ForeignKey(
                        name: "fk_activities_destinations_destination_id",
                        column: x => x.destination_id,
                        principalTable: "destinations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transportations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    price_value = table.Column<decimal>(type: "numeric(9,2)", precision: 9, scale: 2, nullable: true),
                    price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    arrival_destination_id = table.Column<Guid>(type: "uuid", nullable: true),
                    departure_destination_id = table.Column<Guid>(type: "uuid", nullable: true),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transportations", x => x.id);
                    table.ForeignKey(
                        name: "fk_transportations_destinations_arrival_destination_id",
                        column: x => x.arrival_destination_id,
                        principalTable: "destinations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_transportations_destinations_departure_destination_id",
                        column: x => x.departure_destination_id,
                        principalTable: "destinations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_transportations_trips_trip_id",
                        column: x => x.trip_id,
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "legs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    departure_place_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    arrival_place_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    departure_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    arrival_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    price_value = table.Column<decimal>(type: "numeric(9,2)", precision: 9, scale: 2, nullable: true),
                    price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    company = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    service_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    reservation = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    seat = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    transportation_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_legs", x => x.id);
                    table.ForeignKey(
                        name: "fk_legs_transportations_transportation_id",
                        column: x => x.transportation_id,
                        principalTable: "transportations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_accommodations_destination_id",
                table: "accommodations",
                column: "destination_id");

            migrationBuilder.CreateIndex(
                name: "ix_activities_destination_id",
                table: "activities",
                column: "destination_id");

            migrationBuilder.CreateIndex(
                name: "ix_destinations_trip_id",
                table: "destinations",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "ix_legs_transportation_id",
                table: "legs",
                column: "transportation_id");

            migrationBuilder.CreateIndex(
                name: "ix_transportations_arrival_destination_id",
                table: "transportations",
                column: "arrival_destination_id");

            migrationBuilder.CreateIndex(
                name: "ix_transportations_departure_destination_id",
                table: "transportations",
                column: "departure_destination_id");

            migrationBuilder.CreateIndex(
                name: "ix_transportations_trip_id",
                table: "transportations",
                column: "trip_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accommodations");

            migrationBuilder.DropTable(
                name: "activities");

            migrationBuilder.DropTable(
                name: "legs");

            migrationBuilder.DropTable(
                name: "transportations");

            migrationBuilder.DropTable(
                name: "destinations");

            migrationBuilder.DropTable(
                name: "trips");
        }
    }
}
