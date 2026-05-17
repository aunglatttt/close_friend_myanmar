using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloseFriendMyanamr.Migrations
{
    /// <inheritdoc />
    public partial class AddTownshipSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Township",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Township",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Township");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Township");
        }
    }
}
