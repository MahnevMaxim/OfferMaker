using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class DescriptionRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 1, 14, 15, 55, 8, 380, DateTimeKind.Utc).AddTicks(6947),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 1, 12, 2, 56, 45, 153, DateTimeKind.Utc).AddTicks(7994));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2022, 1, 14, 15, 55, 8, 377, DateTimeKind.Utc).AddTicks(2539),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 1, 12, 2, 56, 45, 148, DateTimeKind.Utc).AddTicks(5966));

            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                table: "Nomenclatures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 1, 12, 2, 56, 45, 153, DateTimeKind.Utc).AddTicks(7994),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 1, 14, 15, 55, 8, 380, DateTimeKind.Utc).AddTicks(6947));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2022, 1, 12, 2, 56, 45, 148, DateTimeKind.Utc).AddTicks(5966),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 1, 14, 15, 55, 8, 377, DateTimeKind.Utc).AddTicks(2539));

            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                table: "Nomenclatures",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
