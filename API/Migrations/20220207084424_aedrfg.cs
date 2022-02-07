using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class aedrfg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "OfferTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 2, 7, 8, 44, 23, 994, DateTimeKind.Utc).AddTicks(6117),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 2, 5, 11, 43, 49, 917, DateTimeKind.Utc).AddTicks(2648));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 2, 7, 8, 44, 23, 991, DateTimeKind.Utc).AddTicks(7170),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 2, 5, 11, 43, 49, 912, DateTimeKind.Utc).AddTicks(8301));

            migrationBuilder.AddColumn<string>(
                name: "OldIdentifer",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2022, 2, 7, 8, 44, 23, 985, DateTimeKind.Utc).AddTicks(9891),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 2, 5, 11, 43, 49, 906, DateTimeKind.Utc).AddTicks(7841));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OldIdentifer",
                table: "Offers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "OfferTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 2, 5, 11, 43, 49, 917, DateTimeKind.Utc).AddTicks(2648),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 2, 7, 8, 44, 23, 994, DateTimeKind.Utc).AddTicks(6117));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 2, 5, 11, 43, 49, 912, DateTimeKind.Utc).AddTicks(8301),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 2, 7, 8, 44, 23, 991, DateTimeKind.Utc).AddTicks(7170));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2022, 2, 5, 11, 43, 49, 906, DateTimeKind.Utc).AddTicks(7841),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 2, 7, 8, 44, 23, 985, DateTimeKind.Utc).AddTicks(9891));
        }
    }
}
