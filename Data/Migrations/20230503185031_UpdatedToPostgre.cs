using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemoteWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedToPostgre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "ResponseFileData",
                table: "Issues",
                type: "bytea",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "BYTEA",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "ResponseFileData",
                table: "Issues",
                type: "BYTEA",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldNullable: true);
        }
    }
}
