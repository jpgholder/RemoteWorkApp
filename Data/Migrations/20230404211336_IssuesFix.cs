using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemoteWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class IssuesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Issues",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Issues",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResponderId",
                table: "Issues",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeamId",
                table: "Issues",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Issues",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_ResponderId",
                table: "Issues",
                column: "ResponderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_AspNetUsers_ResponderId",
                table: "Issues",
                column: "ResponderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_AspNetUsers_ResponderId",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_ResponderId",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "ResponderId",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Issues");
        }
    }
}
