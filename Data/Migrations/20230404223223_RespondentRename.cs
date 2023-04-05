using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemoteWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class RespondentRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_AspNetUsers_ResponderId",
                table: "Issues");

            migrationBuilder.RenameColumn(
                name: "ResponderId",
                table: "Issues",
                newName: "RespondentId");

            migrationBuilder.RenameIndex(
                name: "IX_Issues_ResponderId",
                table: "Issues",
                newName: "IX_Issues_RespondentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_AspNetUsers_RespondentId",
                table: "Issues",
                column: "RespondentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_AspNetUsers_RespondentId",
                table: "Issues");

            migrationBuilder.RenameColumn(
                name: "RespondentId",
                table: "Issues",
                newName: "ResponderId");

            migrationBuilder.RenameIndex(
                name: "IX_Issues_RespondentId",
                table: "Issues",
                newName: "IX_Issues_ResponderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_AspNetUsers_ResponderId",
                table: "Issues",
                column: "ResponderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
