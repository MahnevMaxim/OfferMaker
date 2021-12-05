using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class New2335 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Users_ManagerId",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Users_OfferCreatorId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_ManagerId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_OfferCreatorId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "OfferCreatorId",
                table: "Offers");

            migrationBuilder.AddColumn<string>(
                name: "Manager",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfferCreator",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Manager",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "OfferCreator",
                table: "Offers");

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "Offers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OfferCreatorId",
                table: "Offers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Offers_ManagerId",
                table: "Offers",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_OfferCreatorId",
                table: "Offers",
                column: "OfferCreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Users_ManagerId",
                table: "Offers",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Users_OfferCreatorId",
                table: "Offers",
                column: "OfferCreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
