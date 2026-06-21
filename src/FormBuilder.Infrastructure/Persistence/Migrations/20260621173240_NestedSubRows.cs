using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NestedSubRows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "PageId",
                table: "FormRows",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentColumnId",
                table: "FormRows",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormRows_ParentColumnId",
                table: "FormRows",
                column: "ParentColumnId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormRows_FormColumns_ParentColumnId",
                table: "FormRows",
                column: "ParentColumnId",
                principalTable: "FormColumns",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormRows_FormColumns_ParentColumnId",
                table: "FormRows");

            migrationBuilder.DropIndex(
                name: "IX_FormRows_ParentColumnId",
                table: "FormRows");

            migrationBuilder.DropColumn(
                name: "ParentColumnId",
                table: "FormRows");

            migrationBuilder.AlterColumn<Guid>(
                name: "PageId",
                table: "FormRows",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
