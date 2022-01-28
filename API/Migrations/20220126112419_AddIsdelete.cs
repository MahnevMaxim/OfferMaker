using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class AddIsdelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "OfferTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 1, 26, 11, 24, 18, 919, DateTimeKind.Utc).AddTicks(2322),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 1, 22, 5, 8, 27, 241, DateTimeKind.Utc).AddTicks(2326));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 1, 26, 11, 24, 18, 915, DateTimeKind.Utc).AddTicks(9933),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 1, 22, 5, 8, 27, 237, DateTimeKind.Utc).AddTicks(5417));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2022, 1, 26, 11, 24, 18, 911, DateTimeKind.Utc).AddTicks(2454),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 1, 22, 5, 8, 27, 231, DateTimeKind.Utc).AddTicks(9649));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Banners",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Advertisings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Advertisings");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "OfferTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 1, 22, 5, 8, 27, 241, DateTimeKind.Utc).AddTicks(2326),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 1, 26, 11, 24, 18, 919, DateTimeKind.Utc).AddTicks(2322));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 1, 22, 5, 8, 27, 237, DateTimeKind.Utc).AddTicks(5417),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 1, 26, 11, 24, 18, 915, DateTimeKind.Utc).AddTicks(9933));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2022, 1, 22, 5, 8, 27, 231, DateTimeKind.Utc).AddTicks(9649),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 1, 26, 11, 24, 18, 911, DateTimeKind.Utc).AddTicks(2454));
        }
    }
}
