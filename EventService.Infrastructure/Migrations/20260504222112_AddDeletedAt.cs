using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<DateTime>(
                name: "DELETED_AT",
                table: "EVENEMENTS",
                type: "TIMESTAMP(7)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DELETED_AT",
                table: "EVENEMENTS");

            migrationBuilder.AddColumn<bool>(
                name: "IS_DELETED",
                table: "EVENEMENTS",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: 0);
        }
    }
}
