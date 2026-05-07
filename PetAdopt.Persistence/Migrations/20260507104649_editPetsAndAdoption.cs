using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetAdopt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class editPetsAndAdoption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Pets",
                newName: "requestStatus");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "AdoptionRequests",
                newName: "RequestStatus");

            migrationBuilder.AddColumn<int>(
                name: "petStatusForAdoption",
                table: "Pets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "postsApprovalStatus",
                table: "Pets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Favorites",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PetStatusForAdoption",
                table: "AdoptionRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "petStatusForAdoption",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "postsApprovalStatus",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "PetStatusForAdoption",
                table: "AdoptionRequests");

            migrationBuilder.RenameColumn(
                name: "requestStatus",
                table: "Pets",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "RequestStatus",
                table: "AdoptionRequests",
                newName: "Status");
        }
    }
}
