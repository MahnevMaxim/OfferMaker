using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class AddPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 12, 27, 23, 46, 33, 909, DateTimeKind.Utc).AddTicks(1678),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 12, 25, 9, 7, 32, 228, DateTimeKind.Utc).AddTicks(1054));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2021, 12, 27, 23, 46, 33, 906, DateTimeKind.Utc).AddTicks(897),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2021, 12, 25, 9, 7, 32, 224, DateTimeKind.Utc).AddTicks(9619));

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Permissions = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Positions_PositionName",
                table: "Positions",
                column: "PositionName",
                unique: true,
                filter: "[PositionName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 12, 25, 9, 7, 32, 228, DateTimeKind.Utc).AddTicks(1054),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 12, 27, 23, 46, 33, 909, DateTimeKind.Utc).AddTicks(1678));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2021, 12, 25, 9, 7, 32, 224, DateTimeKind.Utc).AddTicks(9619),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2021, 12, 27, 23, 46, 33, 906, DateTimeKind.Utc).AddTicks(897));
        }
    }
}
