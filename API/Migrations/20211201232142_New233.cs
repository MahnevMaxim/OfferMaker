using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class New233 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Users_OfferCreatorId",
                table: "Offers");

            migrationBuilder.AlterColumn<int>(
                name: "OfferCreatorId",
                table: "Offers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Users_OfferCreatorId",
                table: "Offers",
                column: "OfferCreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Users_OfferCreatorId",
                table: "Offers");

            migrationBuilder.AlterColumn<int>(
                name: "OfferCreatorId",
                table: "Offers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Users_OfferCreatorId",
                table: "Offers",
                column: "OfferCreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
