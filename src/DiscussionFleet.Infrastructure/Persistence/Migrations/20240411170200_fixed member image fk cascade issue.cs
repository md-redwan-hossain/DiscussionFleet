using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscussionFleet.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class fixedmemberimagefkcascadeissue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_MultimediaImages_ProfileImageId",
                table: "Members");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_MultimediaImages_ProfileImageId",
                table: "Members",
                column: "ProfileImageId",
                principalTable: "MultimediaImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_MultimediaImages_ProfileImageId",
                table: "Members");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_MultimediaImages_ProfileImageId",
                table: "Members",
                column: "ProfileImageId",
                principalTable: "MultimediaImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
