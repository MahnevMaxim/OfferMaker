using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class New2335928 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Offers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 12, 2, 3, 27, 36, 555, DateTimeKind.Utc).AddTicks(1577),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 12, 2, 3, 12, 7, 652, DateTimeKind.Utc).AddTicks(9850));

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Offers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2021, 12, 2, 3, 27, 36, 546, DateTimeKind.Utc).AddTicks(9557),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2021, 12, 2, 3, 12, 7, 639, DateTimeKind.Utc).AddTicks(3231));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Offers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 12, 2, 3, 12, 7, 652, DateTimeKind.Utc).AddTicks(9850),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 12, 2, 3, 27, 36, 555, DateTimeKind.Utc).AddTicks(1577));

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2021, 12, 2, 3, 12, 7, 639, DateTimeKind.Utc).AddTicks(3231),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2021, 12, 2, 3, 27, 36, 546, DateTimeKind.Utc).AddTicks(9557));
        }
    }
}
