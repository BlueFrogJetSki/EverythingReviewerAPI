using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFinderAPI.Migrations
{
    /// <inheritdoc />
    public partial class RatingCountandRatingSum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RatingCount",
                table: "Items",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "RatingSum",
                table: "Items",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatingCount",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "RatingSum",
                table: "Items");
        }
    }
}
