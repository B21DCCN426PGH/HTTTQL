using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace ThietBiDienTu_2.Models;

public partial class ToolDbContext : DbContext
{
    public ToolDbContext()
    {
    }

    public ToolDbContext(DbContextOptions<ToolDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chitietphieumuon> Chitietphieumuons { get; set; }

    public virtual DbSet<Coso> Cosos { get; set; }

    public virtual DbSet<Dongthietbi> Dongthietbis { get; set; }

    public virtual DbSet<Khoa> Khoas { get; set; }

    public virtual DbSet<Nganh> Nganhs { get; set; }

    public virtual DbSet<Nhanvien> Nhanviens { get; set; }

    public virtual DbSet<Phieumuon> Phieumuons { get; set; }

    public virtual DbSet<Phieusua> Phieusuas { get; set; }

    public virtual DbSet<Phong> Phongs { get; set; }

    public virtual DbSet<Sinhvien> Sinhviens { get; set; }

    public virtual DbSet<Taikhoan> Taikhoans { get; set; }

    public virtual DbSet<Thietbi> Thietbis { get; set; }

    public virtual DbSet<Chitietphieusua> Chitietphieusuas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=MICHAEL;Initial Catalog=QLTHIETBI2;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chitietphieumuon>(entity =>
        {
            entity.HasKey(e => new { e.Mapm, e.Matb });

            entity.ToTable("CHITIETPHIEUMUON");

            entity.HasIndex(e => e.Matb, "IX_CHITIETPHIEUMUON_IDCTTB");

            entity.Property(e => e.Mapm).HasColumnName("MAPM");
            entity.Property(e => e.Matb).HasColumnName("MATB");
            entity.Property(e => e.Ngaytra)
                .HasColumnType("datetime")
                .HasColumnName("NGAYTRA");

            entity.HasOne(d => d.MapmNavigation).WithMany(p => p.Chitietphieumuons)
                .HasForeignKey(d => d.Mapm)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CHITIETPHIEUMUON_PHIEUMUON1");

            entity.HasOne(d => d.MatbNavigation).WithMany(p => p.Chitietphieumuons)
                .HasForeignKey(d => d.Matb)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CHITIETPHIEUMUON_CHITIETTHIETBI");
        });

        modelBuilder.Entity<Chitietphieusua>(entity =>
        {
            entity.HasKey(e => new { e.Maps, e.Matb });

            entity.ToTable("CHITIETPHIEUSUA");

            entity.HasIndex(e => e.Matb, "IX_CHITIETPHIEUSUA_IDCTTB");

            entity.Property(e => e.Maps).HasColumnName("MAPS");
            entity.Property(e => e.Matb).HasColumnName("MATB");
            entity.Property(e => e.Mota).HasColumnName("MOTA");
            entity.Property(e => e.Chiphi)
                .HasColumnType("money")
                .HasColumnName("CHIPHI");

            entity.Property(e => e.Ngayhoanthanh)
                .HasColumnType("datetime")
                .HasColumnName("NGAYHOANTHANH");

            entity.HasOne(d => d.MapsNavigation).WithMany(p => p.Chitietphieusuas)
                .HasForeignKey(d => d.Maps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CHITIETPHIEUSUA_PHIEUSUA1");

            entity.HasOne(d => d.MatbNavigation).WithMany(p => p.Chitietphieusuas)
                .HasForeignKey(d => d.Matb)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CHITIETPHIEUSUA_CHITIETTHIETBI");
        });

        modelBuilder.Entity<Coso>(entity =>
        {
            entity.HasKey(e => e.Macs).HasName("PK__COSO__603F183B80D63A70");

            entity.ToTable("COSO");

            entity.Property(e => e.Macs).HasColumnName("MACS");
            entity.Property(e => e.Diachi)
                .HasMaxLength(255)
                .HasColumnName("DIACHI");
            entity.Property(e => e.Tencs)
                .HasMaxLength(255)
                .HasColumnName("TENCS");
        });

        modelBuilder.Entity<Dongthietbi>(entity =>
        {
            entity.HasKey(e => e.Madongtb).HasName("PK__THIETBI__6023721DFE47CEB0");

            entity.ToTable("DONGTHIETBI");

            entity.Property(e => e.Madongtb).HasColumnName("MADONGTB");
            entity.Property(e => e.Hinhanh).HasColumnName("HINHANH");
            entity.Property(e => e.Mota).HasColumnName("MOTA");
            entity.Property(e => e.Soluong).HasColumnName("SOLUONG");
            entity.Property(e => e.Tendongtb)
                .HasMaxLength(255)
                .HasColumnName("TENDONGTB");
        });

        modelBuilder.Entity<Khoa>(entity =>
        {
            entity.HasKey(e => e.Makhoa);

            entity.ToTable("KHOA");

            entity.Property(e => e.Makhoa).HasColumnName("MAKHOA");
            entity.Property(e => e.Tenkhoa)
                .HasMaxLength(55)
                .HasColumnName("TENKHOA");
        });

        modelBuilder.Entity<Nganh>(entity =>
        {
            entity.HasKey(e => e.Manganh);

            entity.ToTable("NGANH");

            entity.Property(e => e.Manganh)
            .HasColumnName("MANGANH");
            entity.Property(e => e.Tennganh)
                .HasMaxLength(255)
                .HasColumnName("TENNGANH");
        });

        modelBuilder.Entity<Nhanvien>(entity =>
        {
            entity.HasKey(e => e.Manv).HasName("PK__NHANVIEN__603F511441B78137");

            entity.ToTable("NHANVIEN");

            entity.Property(e => e.Manv)
                .ValueGeneratedNever()
                .HasColumnName("MANV");
            entity.Property(e => e.Diachi)
                .HasMaxLength(255)
                .HasColumnName("DIACHI");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Gioitinh)
                .HasMaxLength(3)
                .HasColumnName("GIOITINH");
            entity.Property(e => e.Ngaysinh)
                .HasColumnType("datetime")
                .HasColumnName("NGAYSINH");
            entity.Property(e => e.Sdt)
                .HasMaxLength(255)
                .HasColumnName("SDT");
            entity.Property(e => e.Tennv)
                .HasMaxLength(255)
                .HasColumnName("TENNV");

            entity.HasOne(d => d.ManvNavigation).WithOne(p => p.Nhanvien)
                .HasForeignKey<Nhanvien>(d => d.Manv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ACCOUNT__NV");
        });

        modelBuilder.Entity<Phieumuon>(entity =>
        {
            entity.HasKey(e => e.Mapm).HasName("PK__PHIEUMUO__603F61CDEBB7FD91");

            entity.ToTable("PHIEUMUON");

            entity.HasIndex(e => e.Manv, "IX_PHIEUMUON_MANV");

            entity.HasIndex(e => e.Masv, "IX_PHIEUMUON_MASV");

            entity.Property(e => e.Mapm).HasColumnName("MAPM");
            entity.Property(e => e.Lydomuon)
                .HasDefaultValueSql("(N'')")
                .HasColumnName("LYDOMUON");
            entity.Property(e => e.Manv).HasColumnName("MANV");
            entity.Property(e => e.Masv).HasColumnName("MASV");
            entity.Property(e => e.LydoTuChoi)
               .HasMaxLength(255)
               .HasColumnName("LYDOTUCHOI");
            entity.Property(e => e.LydoHuy)
               .HasMaxLength(255)
               .HasColumnName("LYDOHUY");

            entity.Property(e => e.Ngaylap)
                .HasColumnType("datetime")
                .HasColumnName("NGAYLAP");
            entity.Property(e => e.Ngaymuon)
                .HasColumnType("datetime")
                .HasColumnName("NGAYMUON");
            entity.Property(e => e.Trangthai)
                .HasColumnName("TRANGTHAI");

            entity.HasOne(d => d.ManvNavigation).WithMany(p => p.Phieumuons)
                .HasForeignKey(d => d.Manv)
                .HasConstraintName("FK_PHIEUMUON_NHANVIEN1");

            entity.HasOne(d => d.MasvNavigation).WithMany(p => p.Phieumuons)
                .HasForeignKey(d => d.Masv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PHIEUMUON_SINHVIEN");
        });

        modelBuilder.Entity<Phieusua>(entity =>
        {
            entity.HasKey(e => e.Maps);

            entity.ToTable("PHIEUSUA");

            entity.Property(e => e.Maps).HasColumnName("MAPS");
            entity.Property(e => e.Tongchiphi)
                .HasColumnType("money")
                .HasColumnName("TONGCHIPHI");
            entity.Property(e => e.Ngaylap)
                .HasColumnType("datetime")
                .HasColumnName("NGAYLAP");
            entity.Property(e => e.Trangthai).HasColumnName("TRANGTHAI");
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.Map).HasName("PK__PHONG__C790778043DD78DD");

            entity.ToTable("PHONG");

            entity.HasIndex(e => e.Macs, "IX_PHONG_MACS");

            entity.Property(e => e.Map)
                .HasMaxLength(30)
                .HasColumnName("MAP");
            entity.Property(e => e.Douutien).HasColumnName("DOUUTIEN");
            entity.Property(e => e.Loaiphong)
                .HasMaxLength(255)
                .HasColumnName("LOAIPHONG");
            entity.Property(e => e.Macs).HasColumnName("MACS");
            entity.Property(e => e.Tenphong)
                .HasMaxLength(255)
                .HasColumnName("TENPHONG");

            entity.HasOne(d => d.MacsNavigation).WithMany(p => p.Phongs)
                .HasForeignKey(d => d.Macs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHONG__MACS__5165187F");
        });

        modelBuilder.Entity<Sinhvien>(entity =>
        {
            entity.HasKey(e => e.Masv).HasName("PK__SINHVIEN__60228A2812E98CDF");

            entity.ToTable("SINHVIEN");

            entity.Property(e => e.Masv)
                .ValueGeneratedNever()
                .HasColumnName("MASV");
            entity.Property(e => e.Diachi)
                .HasMaxLength(255)
                .HasColumnName("DIACHI");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Gioitinh)
                .HasMaxLength(3)
                .HasDefaultValueSql("(N'')")
                .HasColumnName("GIOITINH");
            entity.Property(e => e.Makhoa).HasColumnName("MAKHOA");
            entity.Property(e => e.Manganh).HasColumnName("MANGANH");
            entity.Property(e => e.Ngaysinh)
                .HasColumnType("datetime")
                .HasColumnName("NGAYSINH");
            entity.Property(e => e.Sdt)
                .HasMaxLength(255)
                .HasColumnName("SDT");
            entity.Property(e => e.Tensv)
                .HasMaxLength(255)
                .HasColumnName("TENSV");

            entity.HasOne(d => d.MakhoaNavigation).WithMany(p => p.Sinhviens)
                .HasForeignKey(d => d.Makhoa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SINHVIEN_KHOA");

            entity.HasOne(d => d.ManganhNavigation).WithMany(p => p.Sinhviens)
                .HasForeignKey(d => d.Manganh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SINHVIEN_NGANH");

            entity.HasOne(d => d.MasvNavigation).WithOne(p => p.Sinhvien)
                .HasForeignKey<Sinhvien>(d => d.Masv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ACCOUNT__SV");
        });

        modelBuilder.Entity<Taikhoan>(entity =>
        {
            entity.HasKey(e => e.Matk).HasName("PK__ACCOUNT__6029121DD1D4BE32");

            entity.ToTable("TAIKHOAN");

            entity.Property(e => e.Matk)
                .ValueGeneratedNever()
                .HasColumnName("MATK");
            entity.Property(e => e.Loaitk).HasColumnName("LOAITK");
            entity.Property(e => e.Matkhau)
                .HasMaxLength(255)
                .HasColumnName("MATKHAU");
        });

        modelBuilder.Entity<Thietbi>(entity =>
        {
            entity.HasKey(e => e.Matb).HasName("PK_CHITIETTHIETBI");

            entity.ToTable("THIETBI");

            entity.HasIndex(e => e.Map, "IX_CHITIETTHIETBI_MAP");

            entity.HasIndex(e => e.Madongtb, "IX_CHITIETTHIETBI_MATB");

            entity.Property(e => e.Matb).HasColumnName("MATB");
            entity.Property(e => e.Madongtb).HasColumnName("MADONGTB");
            entity.Property(e => e.Map)
                .HasMaxLength(30)
                .HasColumnName("MAP");
            entity.Property(e => e.Seri)
                .HasMaxLength(255)
                .HasColumnName("SERI");
            entity.Property(e => e.Trangthai)
                .HasMaxLength(255)
                .HasColumnName("TRANGTHAI");

            entity.HasOne(d => d.MadongtbNavigation).WithMany(p => p.Thietbis)
                .HasForeignKey(d => d.Madongtb)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CHITIETTHIETBI_THIETBI");

            entity.HasOne(d => d.MapNavigation).WithMany(p => p.Thietbis)
                .HasForeignKey(d => d.Map)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CHITIETTHIETBI_PHONG");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
