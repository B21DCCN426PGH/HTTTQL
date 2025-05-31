using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThietBiDienTu_2.Migrations
{
    /// <inheritdoc />
    public partial class v0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "COSO",
                columns: table => new
                {
                    MACS = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TENCS = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DIACHI = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__COSO__603F183B80D63A70", x => x.MACS);
                });

            migrationBuilder.CreateTable(
                name: "DONGTHIETBI",
                columns: table => new
                {
                    MADONGTB = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TENDONGTB = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HINHANH = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SOLUONG = table.Column<int>(type: "int", nullable: false),
                    MOTA = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__THIETBI__6023721DFE47CEB0", x => x.MADONGTB);
                });

            migrationBuilder.CreateTable(
                name: "KHOA",
                columns: table => new
                {
                    MAKHOA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TENKHOA = table.Column<string>(type: "nvarchar(55)", maxLength: 55, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KHOA", x => x.MAKHOA);
                });

            migrationBuilder.CreateTable(
                name: "NGANH",
                columns: table => new
                {
                    MANGANH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TENNGANH = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NGANH", x => x.MANGANH);
                });

            migrationBuilder.CreateTable(
                name: "TAIKHOAN",
                columns: table => new
                {
                    MATK = table.Column<int>(type: "int", nullable: false),
                    MATKHAU = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LOAITK = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ACCOUNT__6029121DD1D4BE32", x => x.MATK);
                });

            migrationBuilder.CreateTable(
                name: "PHONG",
                columns: table => new
                {
                    MAP = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MACS = table.Column<int>(type: "int", nullable: false),
                    TENPHONG = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LOAIPHONG = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DOUUTIEN = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PHONG__C790778043DD78DD", x => x.MAP);
                    table.ForeignKey(
                        name: "FK__PHONG__MACS__5165187F",
                        column: x => x.MACS,
                        principalTable: "COSO",
                        principalColumn: "MACS");
                });

            migrationBuilder.CreateTable(
                name: "NHANVIEN",
                columns: table => new
                {
                    MANV = table.Column<int>(type: "int", nullable: false),
                    TENNV = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    GIOITINH = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    NGAYSINH = table.Column<DateTime>(type: "datetime", nullable: false),
                    EMAIL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DIACHI = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NHANVIEN__603F511441B78137", x => x.MANV);
                    table.ForeignKey(
                        name: "FK__ACCOUNT__NV",
                        column: x => x.MANV,
                        principalTable: "TAIKHOAN",
                        principalColumn: "MATK");
                });

            migrationBuilder.CreateTable(
                name: "SINHVIEN",
                columns: table => new
                {
                    MASV = table.Column<int>(type: "int", nullable: false),
                    TENSV = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MAKHOA = table.Column<int>(type: "int", nullable: false),
                    MANGANH = table.Column<int>(type: "int", nullable: false),
                    EMAIL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NGAYSINH = table.Column<DateTime>(type: "datetime", nullable: false),
                    DIACHI = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GIOITINH = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValueSql: "(N'')"),
                    DIEMRENLUYEN = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SINHVIEN__60228A2812E98CDF", x => x.MASV);
                    table.ForeignKey(
                        name: "FK_SINHVIEN_KHOA",
                        column: x => x.MAKHOA,
                        principalTable: "KHOA",
                        principalColumn: "MAKHOA");
                    table.ForeignKey(
                        name: "FK_SINHVIEN_NGANH",
                        column: x => x.MANGANH,
                        principalTable: "NGANH",
                        principalColumn: "MANGANH");
                    table.ForeignKey(
                        name: "FK__ACCOUNT__SV",
                        column: x => x.MASV,
                        principalTable: "TAIKHOAN",
                        principalColumn: "MATK");
                });

            migrationBuilder.CreateTable(
                name: "THIETBI",
                columns: table => new
                {
                    MATB = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SERI = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MAP = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MADONGTB = table.Column<int>(type: "int", nullable: false),
                    TRANGTHAI = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHITIETTHIETBI", x => x.MATB);
                    table.ForeignKey(
                        name: "FK_CHITIETTHIETBI_PHONG",
                        column: x => x.MAP,
                        principalTable: "PHONG",
                        principalColumn: "MAP");
                    table.ForeignKey(
                        name: "FK_CHITIETTHIETBI_THIETBI",
                        column: x => x.MADONGTB,
                        principalTable: "DONGTHIETBI",
                        principalColumn: "MADONGTB");
                });

            migrationBuilder.CreateTable(
                name: "PHIEUMUON",
                columns: table => new
                {
                    MAPM = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NGAYMUON = table.Column<DateTime>(type: "datetime", nullable: true),
                    NGAYLAP = table.Column<DateTime>(type: "datetime", nullable: false),
                    MANV = table.Column<int>(type: "int", nullable: true),
                    MASV = table.Column<int>(type: "int", nullable: false),
                    TRANGTHAI = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LYDOMUON = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "(N'')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PHIEUMUO__603F61CDEBB7FD91", x => x.MAPM);
                    table.ForeignKey(
                        name: "FK_PHIEUMUON_NHANVIEN1",
                        column: x => x.MANV,
                        principalTable: "NHANVIEN",
                        principalColumn: "MANV");
                    table.ForeignKey(
                        name: "FK_PHIEUMUON_SINHVIEN",
                        column: x => x.MASV,
                        principalTable: "SINHVIEN",
                        principalColumn: "MASV");
                });

            migrationBuilder.CreateTable(
                name: "PHIEUSUA",
                columns: table => new
                {
                    MAPS = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MATB = table.Column<int>(type: "int", nullable: false),
                    NGAYLAP = table.Column<DateTime>(type: "datetime", nullable: false),
                    NGAYHOANTAT = table.Column<DateTime>(type: "datetime", nullable: true),
                    CHIPHI = table.Column<decimal>(type: "money", nullable: true),
                    MOTA = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PHIEUSUA", x => x.MAPS);
                    table.ForeignKey(
                        name: "FK_PHIEUSUA_THIETBI",
                        column: x => x.MATB,
                        principalTable: "THIETBI",
                        principalColumn: "MATB");
                });

            migrationBuilder.CreateTable(
                name: "CHITIETPHIEUMUON",
                columns: table => new
                {
                    MATB = table.Column<int>(type: "int", nullable: false),
                    MAPM = table.Column<int>(type: "int", nullable: false),
                    NGAYTRA = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHITIETPHIEUMUON", x => new { x.MAPM, x.MATB });
                    table.ForeignKey(
                        name: "FK_CHITIETPHIEUMUON_CHITIETTHIETBI",
                        column: x => x.MATB,
                        principalTable: "THIETBI",
                        principalColumn: "MATB");
                    table.ForeignKey(
                        name: "FK_CHITIETPHIEUMUON_PHIEUMUON1",
                        column: x => x.MAPM,
                        principalTable: "PHIEUMUON",
                        principalColumn: "MAPM");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CHITIETPHIEUMUON_IDCTTB",
                table: "CHITIETPHIEUMUON",
                column: "MATB");

            migrationBuilder.CreateIndex(
                name: "IX_PHIEUMUON_MANV",
                table: "PHIEUMUON",
                column: "MANV");

            migrationBuilder.CreateIndex(
                name: "IX_PHIEUMUON_MASV",
                table: "PHIEUMUON",
                column: "MASV");

            migrationBuilder.CreateIndex(
                name: "IX_PHIEUSUA_MATB",
                table: "PHIEUSUA",
                column: "MATB");

            migrationBuilder.CreateIndex(
                name: "IX_PHONG_MACS",
                table: "PHONG",
                column: "MACS");

            migrationBuilder.CreateIndex(
                name: "IX_SINHVIEN_MAKHOA",
                table: "SINHVIEN",
                column: "MAKHOA");

            migrationBuilder.CreateIndex(
                name: "IX_SINHVIEN_MANGANH",
                table: "SINHVIEN",
                column: "MANGANH");

            migrationBuilder.CreateIndex(
                name: "IX_CHITIETTHIETBI_MAP",
                table: "THIETBI",
                column: "MAP");

            migrationBuilder.CreateIndex(
                name: "IX_CHITIETTHIETBI_MATB",
                table: "THIETBI",
                column: "MADONGTB");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CHITIETPHIEUMUON");

            migrationBuilder.DropTable(
                name: "PHIEUSUA");

            migrationBuilder.DropTable(
                name: "PHIEUMUON");

            migrationBuilder.DropTable(
                name: "THIETBI");

            migrationBuilder.DropTable(
                name: "NHANVIEN");

            migrationBuilder.DropTable(
                name: "SINHVIEN");

            migrationBuilder.DropTable(
                name: "PHONG");

            migrationBuilder.DropTable(
                name: "DONGTHIETBI");

            migrationBuilder.DropTable(
                name: "KHOA");

            migrationBuilder.DropTable(
                name: "NGANH");

            migrationBuilder.DropTable(
                name: "TAIKHOAN");

            migrationBuilder.DropTable(
                name: "COSO");
        }
    }
}
