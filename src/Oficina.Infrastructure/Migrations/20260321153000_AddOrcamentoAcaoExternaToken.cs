using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oficina.Infrastructure.Migrations
{
    public partial class AddOrcamentoAcaoExternaToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TokenAcaoExterna",
                table: "Orcamentos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "TokenAcaoExternaExpiraEm",
                table: "Orcamentos",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orcamentos_TokenAcaoExterna",
                table: "Orcamentos",
                column: "TokenAcaoExterna",
                unique: true,
                filter: "[TokenAcaoExterna] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orcamentos_TokenAcaoExterna",
                table: "Orcamentos");

            migrationBuilder.DropColumn(
                name: "TokenAcaoExterna",
                table: "Orcamentos");

            migrationBuilder.DropColumn(
                name: "TokenAcaoExternaExpiraEm",
                table: "Orcamentos");
        }
    }
}
