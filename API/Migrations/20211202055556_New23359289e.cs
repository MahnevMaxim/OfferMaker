using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class New23359289e : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Currencies_CurrencyId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_CurrencyId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Offers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 12, 2, 5, 55, 55, 305, DateTimeKind.Utc).AddTicks(8864),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 12, 2, 5, 36, 49, 107, DateTimeKind.Utc).AddTicks(8104));

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2021, 12, 2, 5, 55, 55, 294, DateTimeKind.Utc).AddTicks(7708),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2021, 12, 2, 5, 36, 49, 98, DateTimeKind.Utc).AddTicks(2512));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Offers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 12, 2, 5, 36, 49, 107, DateTimeKind.Utc).AddTicks(8104),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 12, 2, 5, 55, 55, 305, DateTimeKind.Utc).AddTicks(8864));

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Offers",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2021, 12, 2, 5, 36, 49, 98, DateTimeKind.Utc).AddTicks(2512),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2021, 12, 2, 5, 55, 55, 294, DateTimeKind.Utc).AddTicks(7708));

            migrationBuilder.CreateIndex(
                name: "IX_Offers_CurrencyId",
                table: "Offers",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Currencies_CurrencyId",
                table: "Offers",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
