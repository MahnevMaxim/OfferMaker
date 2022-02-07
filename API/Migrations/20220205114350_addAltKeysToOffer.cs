using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class addAltKeysToOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Guid",
                table: "OfferTemplates",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "OfferTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 2, 5, 11, 43, 49, 917, DateTimeKind.Utc).AddTicks(2648),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 2, 4, 7, 1, 27, 610, DateTimeKind.Utc).AddTicks(4194));

            migrationBuilder.AlterColumn<string>(
                name: "Guid",
                table: "Offers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 2, 5, 11, 43, 49, 912, DateTimeKind.Utc).AddTicks(8301),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 2, 4, 7, 1, 27, 607, DateTimeKind.Utc).AddTicks(4458));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2022, 2, 5, 11, 43, 49, 906, DateTimeKind.Utc).AddTicks(7841),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 2, 4, 7, 1, 27, 602, DateTimeKind.Utc).AddTicks(9900));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_OfferTemplates_Guid",
                table: "OfferTemplates",
                column: "Guid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Offers_Guid",
                table: "Offers",
                column: "Guid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_OfferTemplates_Guid",
                table: "OfferTemplates");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Offers_Guid",
                table: "Offers");

            migrationBuilder.AlterColumn<string>(
                name: "Guid",
                table: "OfferTemplates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "OfferTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 2, 4, 7, 1, 27, 610, DateTimeKind.Utc).AddTicks(4194),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 2, 5, 11, 43, 49, 917, DateTimeKind.Utc).AddTicks(2648));

            migrationBuilder.AlterColumn<string>(
                name: "Guid",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 2, 4, 7, 1, 27, 607, DateTimeKind.Utc).AddTicks(4458),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 2, 5, 11, 43, 49, 912, DateTimeKind.Utc).AddTicks(8301));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChangePriceDate",
                table: "Nomenclatures",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2022, 2, 4, 7, 1, 27, 602, DateTimeKind.Utc).AddTicks(9900),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 2, 5, 11, 43, 49, 906, DateTimeKind.Utc).AddTicks(7841));
        }
    }
}
