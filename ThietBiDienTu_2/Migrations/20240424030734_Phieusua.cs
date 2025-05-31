using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThietBiDienTu_2.Migrations
{
    /// <inheritdoc />
    public partial class Phieusua : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PHIEUSUA_THIETBI",
                table: "PHIEUSUA");

            migrationBuilder.DropColumn(
                name: "MOTA",
                table: "PHIEUSUA");

            migrationBuilder.DropColumn(
                name: "NGAYHOANTAT",
                table: "PHIEUSUA");

            migrationBuilder.RenameColumn(
                name: "MATB",
                table: "PHIEUSUA",
                newName: "TRANGTHAI");

            migrationBuilder.RenameColumn(
                name: "CHIPHI",
                table: "PHIEUSUA",
                newName: "TONGCHIPHI");

            migrationBuilder.CreateTable(
                name: "CHITIETPHIEUSUA",
                columns: table => new
                {
                    MAPS = table.Column<int>(type: "int", nullable: false),
                    MATB = table.Column<int>(type: "int", nullable: false),
                    NGAYHOANTHANH = table.Column<DateTime>(type: "datetime", nullable: true),
                    MOTA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CHIPHI = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHITIETPHIEUSUA", x => new { x.MAPS, x.MATB });
                    table.ForeignKey(
                        name: "FK_CHITIETPHIEUSUA_CHITIETTHIETBI",
                        column: x => x.MATB,
                        principalTable: "THIETBI",
                        principalColumn: "MATB");
                    table.ForeignKey(
                        name: "FK_CHITIETPHIEUSUA_PHIEUSUA1",
                        column: x => x.MAPS,
                        principalTable: "PHIEUSUA",
                        principalColumn: "MAPS");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CHITIETPHIEUSUA_IDCTTB",
                table: "CHITIETPHIEUSUA",
                column: "MATB");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CHITIETPHIEUSUA");

            migrationBuilder.RenameColumn(
                name: "TRANGTHAI",
                table: "PHIEUSUA",
                newName: "MATB");

            migrationBuilder.RenameColumn(
                name: "TONGCHIPHI",
                table: "PHIEUSUA",
                newName: "CHIPHI");

            migrationBuilder.AddColumn<string>(
                name: "MOTA",
                table: "PHIEUSUA",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NGAYHOANTAT",
                table: "PHIEUSUA",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PHIEUSUA_THIETBI",
                table: "PHIEUSUA",
                column: "MATB",
                principalTable: "THIETBI",
                principalColumn: "MATB");
        }
    }
}
