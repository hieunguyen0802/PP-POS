using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCodeSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Payments",
                newName: "ChangeGiven");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountApplied",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountTendered",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "CodeSequences",
                columns: table => new
                {
                    Category = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastValue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeSequences", x => new { x.Category, x.DateKey });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CodeSequences");

            migrationBuilder.DropColumn(
                name: "AmountApplied",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "AmountTendered",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "ChangeGiven",
                table: "Payments",
                newName: "Amount");
        }
    }
}
