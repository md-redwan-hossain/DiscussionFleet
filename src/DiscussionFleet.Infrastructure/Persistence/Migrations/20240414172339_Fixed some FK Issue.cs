using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscussionFleet.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixedsomeFKIssue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Questions_AuthorId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Answers_AnswerGiverId",
                table: "Answers");

            migrationBuilder.AddColumn<bool>(
                name: "HasAcceptedAnswer",
                table: "Questions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "AcceptedAnswers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "AcceptedAnswers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_AuthorId",
                table: "Questions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_AnswerGiverId",
                table: "Answers",
                column: "AnswerGiverId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Questions_AuthorId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Answers_AnswerGiverId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "HasAcceptedAnswer",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "AcceptedAnswers");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "AcceptedAnswers");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_AuthorId",
                table: "Questions",
                column: "AuthorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_AnswerGiverId",
                table: "Answers",
                column: "AnswerGiverId",
                unique: true);
        }
    }
}
