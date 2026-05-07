using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetAdopt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class editPetsAndAdoption1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AdoptionRequests_AdoprerId",
                table: "AdoptionRequests");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionRequests_AdoprerId_PetId",
                table: "AdoptionRequests",
                columns: new[] { "AdoprerId", "PetId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AdoptionRequests_AdoprerId_PetId",
                table: "AdoptionRequests");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionRequests_AdoprerId",
                table: "AdoptionRequests",
                column: "AdoprerId");
        }
    }
}
