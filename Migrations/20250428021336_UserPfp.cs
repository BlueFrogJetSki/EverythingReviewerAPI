using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace reviews4everything.Migrations
{
    /// <inheritdoc />
    public partial class UserPfp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "pfpUrl",
                table: "AspNetUsers",
                newName: "PfpUrl");

            migrationBuilder.AlterColumn<string>(
                name: "PfpUrl",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PfpUrl",
                table: "AspNetUsers",
                newName: "pfpUrl");

            migrationBuilder.AlterColumn<string>(
                name: "pfpUrl",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
