using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class ChangeNomGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NomenclaturesIds",
                table: "NomenclatureGroups");

            migrationBuilder.AddColumn<int>(
                name: "NomenclatureGroupId",
                table: "Nomenclatures",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclatures_NomenclatureGroupId",
                table: "Nomenclatures",
                column: "NomenclatureGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nomenclatures_NomenclatureGroups_NomenclatureGroupId",
                table: "Nomenclatures",
                column: "NomenclatureGroupId",
                principalTable: "NomenclatureGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nomenclatures_NomenclatureGroups_NomenclatureGroupId",
                table: "Nomenclatures");

            migrationBuilder.DropIndex(
                name: "IX_Nomenclatures_NomenclatureGroupId",
                table: "Nomenclatures");

            migrationBuilder.DropColumn(
                name: "NomenclatureGroupId",
                table: "Nomenclatures");

            migrationBuilder.AddColumn<string>(
                name: "NomenclaturesIds",
                table: "NomenclatureGroups",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
