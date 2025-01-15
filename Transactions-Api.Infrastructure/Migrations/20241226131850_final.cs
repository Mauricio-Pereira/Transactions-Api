using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transactions_API.Migrations
{
    /// <inheritdoc />
    public partial class final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "recebedor_cpf",
                table: "transacoes",
                newName: "recebedor_documento");

            migrationBuilder.RenameColumn(
                name: "pagador_cpf",
                table: "transacoes",
                newName: "pagador_documento");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_transacao",
                table: "transacoes",
                type: "timestamp",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "recebedor_documento",
                table: "transacoes",
                newName: "recebedor_cpf");

            migrationBuilder.RenameColumn(
                name: "pagador_documento",
                table: "transacoes",
                newName: "pagador_cpf");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_transacao",
                table: "transacoes",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp");
        }
    }
}
