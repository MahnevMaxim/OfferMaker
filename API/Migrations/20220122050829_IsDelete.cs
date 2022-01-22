using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class IsDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "OfferTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 1, 22, 5, 8, 27, 241, DateTimeKind.Utc).AddTicks(2326),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 1, 21, 10, 1, 56, 362, DateTimeKind.Utc).AddTicks(3722));

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "OfferTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 1, 22, 5, 8, 27, 237, DateTimeKind.Utc).AddTicks(5417),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 1, 21, 10, 1, 56, 359, DateTimeKind.Utc).AddTicks(2986));

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Offers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2022, 1, 22, 5, 8, 27, 231, DateTimeKind.Utc).AddTicks(9649),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 1, 21, 10, 1, 56, 354, DateTimeKind.Utc).AddTicks(7721));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "OfferTemplates");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Offers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "OfferTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 1, 21, 10, 1, 56, 362, DateTimeKind.Utc).AddTicks(3722),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 1, 22, 5, 8, 27, 241, DateTimeKind.Utc).AddTicks(2326));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 1, 21, 10, 1, 56, 359, DateTimeKind.Utc).AddTicks(2986),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 1, 22, 5, 8, 27, 237, DateTimeKind.Utc).AddTicks(5417));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2022, 1, 21, 10, 1, 56, 354, DateTimeKind.Utc).AddTicks(7721),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 1, 22, 5, 8, 27, 231, DateTimeKind.Utc).AddTicks(9649));
        }
    }
}
