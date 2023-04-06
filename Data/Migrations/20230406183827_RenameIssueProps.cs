using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemoteWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameIssueProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Issues",
                newName: "ResponseText");

            migrationBuilder.RenameColumn(
                name: "FileData",
                table: "Issues",
                newName: "ResponseFileData");

            migrationBuilder.AddColumn<string>(
                name: "ResponseFileName",
                table: "Issues",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponseFileName",
                table: "Issues");

            migrationBuilder.RenameColumn(
                name: "ResponseText",
                table: "Issues",
                newName: "FileName");

            migrationBuilder.RenameColumn(
                name: "ResponseFileData",
                table: "Issues",
                newName: "FileData");
        }
    }
}
