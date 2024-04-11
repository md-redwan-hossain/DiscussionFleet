using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscussionFleet.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updatedmultimediaimageentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Purpose",
                table: "MultimediaImages",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_MultimediaImages_Purpose_Enum",
                table: "MultimediaImages",
                sql: "[Purpose] BETWEEN CAST(1 AS tinyint) AND CAST(3 AS tinyint)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_MultimediaImages_Purpose_Enum",
                table: "MultimediaImages");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "MultimediaImages");
        }
    }
}
