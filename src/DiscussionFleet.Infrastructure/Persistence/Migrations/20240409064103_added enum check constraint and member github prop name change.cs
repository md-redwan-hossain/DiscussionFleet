using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscussionFleet.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addedenumcheckconstraintandmembergithubpropnamechange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GitHubHandle",
                table: "Members",
                newName: "GithubHandle");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ResourceNotifications_NotificationType_Enum",
                table: "ResourceNotifications",
                sql: "[NotificationType] BETWEEN CAST(1 AS tinyint) AND CAST(3 AS tinyint)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ResourceNotifications_NotificationType_Enum",
                table: "ResourceNotifications");

            migrationBuilder.RenameColumn(
                name: "GithubHandle",
                table: "Members",
                newName: "GitHubHandle");
        }
    }
}
