using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasketBet.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class ItemOdds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ItemOdds",
                table: "BetItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemOdds",
                table: "BetItems");
        }
    }
}
