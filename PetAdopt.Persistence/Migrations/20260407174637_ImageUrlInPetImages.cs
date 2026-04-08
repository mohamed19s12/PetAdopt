using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetAdopt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ImageUrlInPetImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "PetImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "PetImages");
        }
    }
}
