using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemoteWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNavProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Messages_TeamId",
                table: "Messages",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_TeamId",
                table: "Issues",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Teams_TeamId",
                table: "Issues",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Teams_TeamId",
                table: "Messages",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Teams_TeamId",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Teams_TeamId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_TeamId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Issues_TeamId",
                table: "Issues");
        }
    }
}
