﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shoppingifybackend.Migrations.Application
{
    /// <inheritdoc />
    public partial class UpdateItemAndCatMig3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Items");
        }
    }
}
