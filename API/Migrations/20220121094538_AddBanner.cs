using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class AddBanner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "OfferTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 1, 21, 9, 45, 34, 514, DateTimeKind.Utc).AddTicks(3852),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 1, 20, 5, 39, 50, 617, DateTimeKind.Utc).AddTicks(7529));

            migrationBuilder.AddColumn<int>(
                name: "Banner_Id",
                table: "OfferTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 1, 21, 9, 45, 34, 511, DateTimeKind.Utc).AddTicks(286),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 1, 20, 5, 39, 50, 614, DateTimeKind.Utc).AddTicks(9063));

            migrationBuilder.AddColumn<int>(
                name: "Banner_Id",
                table: "Offers",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2022, 1, 21, 9, 45, 34, 505, DateTimeKind.Utc).AddTicks(4421),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 1, 20, 5, 39, 50, 610, DateTimeKind.Utc).AddTicks(7523));

            migrationBuilder.CreateIndex(
                name: "IX_OfferTemplates_Banner_Id",
                table: "OfferTemplates",
                column: "Banner_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_Banner_Id",
                table: "Offers",
                column: "Banner_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Banners_Banner_Id",
                table: "Offers",
                column: "Banner_Id",
                principalTable: "Banners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferTemplates_Banners_Banner_Id",
                table: "OfferTemplates",
                column: "Banner_Id",
                principalTable: "Banners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Banners_Banner_Id",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferTemplates_Banners_Banner_Id",
                table: "OfferTemplates");

            migrationBuilder.DropIndex(
                name: "IX_OfferTemplates_Banner_Id",
                table: "OfferTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Offers_Banner_Id",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "Banner_Id",
                table: "OfferTemplates");

            migrationBuilder.DropColumn(
                name: "Banner_Id",
                table: "Offers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "OfferTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 1, 20, 5, 39, 50, 617, DateTimeKind.Utc).AddTicks(7529),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 1, 21, 9, 45, 34, 514, DateTimeKind.Utc).AddTicks(3852));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 1, 20, 5, 39, 50, 614, DateTimeKind.Utc).AddTicks(9063),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 1, 21, 9, 45, 34, 511, DateTimeKind.Utc).AddTicks(286));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2022, 1, 20, 5, 39, 50, 610, DateTimeKind.Utc).AddTicks(7523),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 1, 21, 9, 45, 34, 505, DateTimeKind.Utc).AddTicks(4421));
        }
    }
}
