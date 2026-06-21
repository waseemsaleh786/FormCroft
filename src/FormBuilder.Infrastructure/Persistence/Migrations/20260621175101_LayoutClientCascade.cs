using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LayoutClientCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormColumns_FormRows_RowId",
                table: "FormColumns");

            migrationBuilder.DropForeignKey(
                name: "FK_FormFields_FormColumns_ColumnId",
                table: "FormFields");

            migrationBuilder.DropForeignKey(
                name: "FK_FormPages_Forms_FormId",
                table: "FormPages");

            migrationBuilder.DropForeignKey(
                name: "FK_FormRows_FormPages_PageId",
                table: "FormRows");

            migrationBuilder.AddForeignKey(
                name: "FK_FormColumns_FormRows_RowId",
                table: "FormColumns",
                column: "RowId",
                principalTable: "FormRows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormFields_FormColumns_ColumnId",
                table: "FormFields",
                column: "ColumnId",
                principalTable: "FormColumns",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormPages_Forms_FormId",
                table: "FormPages",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormRows_FormPages_PageId",
                table: "FormRows",
                column: "PageId",
                principalTable: "FormPages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormColumns_FormRows_RowId",
                table: "FormColumns");

            migrationBuilder.DropForeignKey(
                name: "FK_FormFields_FormColumns_ColumnId",
                table: "FormFields");

            migrationBuilder.DropForeignKey(
                name: "FK_FormPages_Forms_FormId",
                table: "FormPages");

            migrationBuilder.DropForeignKey(
                name: "FK_FormRows_FormPages_PageId",
                table: "FormRows");

            migrationBuilder.AddForeignKey(
                name: "FK_FormColumns_FormRows_RowId",
                table: "FormColumns",
                column: "RowId",
                principalTable: "FormRows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormFields_FormColumns_ColumnId",
                table: "FormFields",
                column: "ColumnId",
                principalTable: "FormColumns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormPages_Forms_FormId",
                table: "FormPages",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormRows_FormPages_PageId",
                table: "FormRows",
                column: "PageId",
                principalTable: "FormPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
