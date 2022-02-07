using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class PermissionsRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Permissions",
                table: "Positions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "OfferTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 2, 4, 7, 1, 27, 610, DateTimeKind.Utc).AddTicks(4194),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 2, 3, 15, 42, 50, 516, DateTimeKind.Utc).AddTicks(4602));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 2, 4, 7, 1, 27, 607, DateTimeKind.Utc).AddTicks(4458),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 2, 3, 15, 42, 50, 512, DateTimeKind.Utc).AddTicks(2724));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2022, 2, 4, 7, 1, 27, 602, DateTimeKind.Utc).AddTicks(9900),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 2, 3, 15, 42, 50, 506, DateTimeKind.Utc).AddTicks(6204));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Permissions",
                table: "Positions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "OfferTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 2, 3, 15, 42, 50, 516, DateTimeKind.Utc).AddTicks(4602),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 2, 4, 7, 1, 27, 610, DateTimeKind.Utc).AddTicks(4194));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 2, 3, 15, 42, 50, 512, DateTimeKind.Utc).AddTicks(2724),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 2, 4, 7, 1, 27, 607, DateTimeKind.Utc).AddTicks(4458));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2022, 2, 3, 15, 42, 50, 506, DateTimeKind.Utc).AddTicks(6204),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 2, 4, 7, 1, 27, 602, DateTimeKind.Utc).AddTicks(9900));
        }
    }
}
