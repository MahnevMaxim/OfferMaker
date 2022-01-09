using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class AddHints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 1, 9, 9, 16, 52, 642, DateTimeKind.Utc).AddTicks(6483),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 1, 8, 12, 14, 44, 366, DateTimeKind.Utc).AddTicks(5954));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2022, 1, 9, 9, 16, 52, 639, DateTimeKind.Utc).AddTicks(5844),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 1, 8, 12, 14, 44, 363, DateTimeKind.Utc).AddTicks(4956));

            migrationBuilder.CreateTable(
                name: "Hints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HintString = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hints", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hints");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 1, 8, 12, 14, 44, 366, DateTimeKind.Utc).AddTicks(5954),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 1, 9, 9, 16, 52, 642, DateTimeKind.Utc).AddTicks(6483));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2022, 1, 8, 12, 14, 44, 363, DateTimeKind.Utc).AddTicks(4956),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 1, 9, 9, 16, 52, 639, DateTimeKind.Utc).AddTicks(5844));
        }
    }
}
