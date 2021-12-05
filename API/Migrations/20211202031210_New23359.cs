using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class New23359 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nomenclatures_Categories_CategoryId",
                table: "Nomenclatures");

            migrationBuilder.DropIndex(
                name: "IX_Nomenclatures_CategoryId",
                table: "Nomenclatures");

            migrationBuilder.DropColumn(
                name: "Manager",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "OfferCreator",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "IsPriceActual",
                table: "Nomenclatures");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 12, 2, 3, 12, 7, 652, DateTimeKind.Utc).AddTicks(9850),
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "Offers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OfferCreatorId",
                table: "Offers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2021, 12, 2, 3, 12, 7, 639, DateTimeKind.Utc).AddTicks(3231),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ActualPricePeriod",
                table: "Nomenclatures",
                type: "int",
                nullable: false,
                defaultValue: 30,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "OfferCreatorId",
                table: "Offers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 12, 2, 3, 12, 7, 652, DateTimeKind.Utc).AddTicks(9850));

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

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2021, 12, 2, 3, 12, 7, 639, DateTimeKind.Utc).AddTicks(3231));

            migrationBuilder.AlterColumn<int>(
                name: "ActualPricePeriod",
                table: "Nomenclatures",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 30);

            migrationBuilder.AddColumn<bool>(
                name: "IsPriceActual",
                table: "Nomenclatures",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclatures_CategoryId",
                table: "Nomenclatures",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nomenclatures_Categories_CategoryId",
                table: "Nomenclatures",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
