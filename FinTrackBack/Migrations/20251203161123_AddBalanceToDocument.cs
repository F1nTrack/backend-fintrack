using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinTrackBack.Migrations
{
    /// <inheritdoc />
    public partial class AddBalanceToDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Documents",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Documents");
        }
    }
}
