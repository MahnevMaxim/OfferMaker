using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class AddOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Currencies_CurrencyId",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Customers_CustomerId",
                table: "Offers");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropIndex(
                name: "IX_Offers_CurrencyId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_CustomerId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Offers");

            migrationBuilder.RenameColumn(
                name: "Images",
                table: "Offers",
                newName: "OfferInfoBlocks");

            migrationBuilder.RenameColumn(
                name: "CommercialInJson",
                table: "Offers",
                newName: "OfferGroups");

            migrationBuilder.AddColumn<string>(
                name: "AdvertisingsDown",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdvertisingsUp",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Banner",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Customer",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discount",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCreateByCostPrice",
                table: "Offers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHideNomsPrice",
                table: "Offers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowPriceDetails",
                table: "Offers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ResultSummInRub",
                table: "Offers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdvertisingsDown",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "AdvertisingsUp",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "Banner",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "Customer",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "IsCreateByCostPrice",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "IsHideNomsPrice",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "IsShowPriceDetails",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "ResultSummInRub",
                table: "Offers");

            migrationBuilder.RenameColumn(
                name: "OfferInfoBlocks",
                table: "Offers",
                newName: "Images");

            migrationBuilder.RenameColumn(
                name: "OfferGroups",
                table: "Offers",
                newName: "CommercialInJson");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Offers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Offers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Group_Offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Offers_CurrencyId",
                table: "Offers",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_CustomerId",
                table: "Offers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Group_OfferId",
                table: "Group",
                column: "OfferId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Currencies_CurrencyId",
                table: "Offers",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Customers_CustomerId",
                table: "Offers",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
