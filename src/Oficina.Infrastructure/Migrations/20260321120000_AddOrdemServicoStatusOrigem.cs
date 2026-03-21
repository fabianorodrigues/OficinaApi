using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oficina.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdemServicoStatusOrigem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DataUltimaAtualizacaoStatus",
                table: "OrdensServico",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<int>(
                name: "OrigemUltimaAtualizacaoStatus",
                table: "OrdensServico",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataUltimaAtualizacaoStatus",
                table: "OrdensServico");

            migrationBuilder.DropColumn(
                name: "OrigemUltimaAtualizacaoStatus",
                table: "OrdensServico");
        }
    }
}
