using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class AddPosition2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 12, 28, 22, 49, 38, 255, DateTimeKind.Utc).AddTicks(9740),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 12, 27, 23, 46, 33, 909, DateTimeKind.Utc).AddTicks(1678));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2021, 12, 28, 22, 49, 38, 252, DateTimeKind.Utc).AddTicks(9069),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2021, 12, 27, 23, 46, 33, 906, DateTimeKind.Utc).AddTicks(897));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 12, 27, 23, 46, 33, 909, DateTimeKind.Utc).AddTicks(1678),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 12, 28, 22, 49, 38, 255, DateTimeKind.Utc).AddTicks(9740));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2021, 12, 27, 23, 46, 33, 906, DateTimeKind.Utc).AddTicks(897),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2021, 12, 28, 22, 49, 38, 252, DateTimeKind.Utc).AddTicks(9069));
        }
    }
}
