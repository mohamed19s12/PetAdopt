using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetAdopt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class rmAnimalType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnimalType",
                table: "Pets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnimalType",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
