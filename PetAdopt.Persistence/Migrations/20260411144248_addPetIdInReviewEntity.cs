using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetAdopt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addPetIdInReviewEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PetId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PetId",
                table: "Reviews",
                column: "PetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Pets_PetId",
                table: "Reviews",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Pets_PetId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_PetId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "PetId",
                table: "Reviews");
        }
    }
}
