using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class ffffffff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "OfferTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 2, 28, 8, 30, 51, 635, DateTimeKind.Utc).AddTicks(8005),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 2, 7, 8, 44, 23, 994, DateTimeKind.Utc).AddTicks(6117));

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "OfferTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 2, 28, 8, 30, 51, 631, DateTimeKind.Utc).AddTicks(8884),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 2, 7, 8, 44, 23, 991, DateTimeKind.Utc).AddTicks(7170));

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2022, 2, 28, 8, 30, 51, 624, DateTimeKind.Utc).AddTicks(6077),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 2, 7, 8, 44, 23, 985, DateTimeKind.Utc).AddTicks(9891));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "OfferTemplates");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Offers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "OfferTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 2, 7, 8, 44, 23, 994, DateTimeKind.Utc).AddTicks(6117),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 2, 28, 8, 30, 51, 635, DateTimeKind.Utc).AddTicks(8005));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 2, 7, 8, 44, 23, 991, DateTimeKind.Utc).AddTicks(7170),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 2, 28, 8, 30, 51, 631, DateTimeKind.Utc).AddTicks(8884));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2022, 2, 7, 8, 44, 23, 985, DateTimeKind.Utc).AddTicks(9891),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 2, 28, 8, 30, 51, 624, DateTimeKind.Utc).AddTicks(6077));
        }
    }
}
