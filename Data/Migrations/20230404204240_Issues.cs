using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemoteWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class Issues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Teams_TeamId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_TeamId",
                table: "Messages");

            migrationBuilder.CreateTable(
                name: "Issues",
                columns: table => new
                {
                    IssueId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileName = table.Column<string>(type: "TEXT", nullable: true),
                    FileData = table.Column<byte[]>(type: "BYTEA", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Issues", x => x.IssueId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Issues");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TeamId",
                table: "Messages",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Teams_TeamId",
                table: "Messages",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
