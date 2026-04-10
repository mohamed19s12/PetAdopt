using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetAdopt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeletePet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionRequests_Pets_PetId",
                table: "AdoptionRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Pets_PetId",
                table: "Favorites");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionRequests_Pets_PetId",
                table: "AdoptionRequests",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Pets_PetId",
                table: "Favorites",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionRequests_Pets_PetId",
                table: "AdoptionRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Pets_PetId",
                table: "Favorites");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionRequests_Pets_PetId",
                table: "AdoptionRequests",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Pets_PetId",
                table: "Favorites",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
