using _1721001086_PanyasakKhamkeuth_Week8.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _1721001086_PanyasakKhamkeuth_Week8.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<TheLoai> TheLoai { get; set; }
        public DbSet<SanPham> SanPham { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<GioHang> GioHang { get; set; }
        public DbSet<SanphamTheLoai> sanphamTheloais { get; set; }
        public DbSet<ProductImage> productImages { get; set; }
        public DbSet<HoaDon> HoaDon { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDon { get; set; }
        public DbSet<NhaCungCap> NhaCungCap { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<FavorProduct> FavorProducts { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<SmtpEmailSenderOptions> SmtpEmailSenderOptions { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<SanphamTheLoai>()
                .HasKey(st => new { st.SanphamId, st.TheloaiId });

            // Các cấu hình khác
            modelBuilder.Entity<ProductImage>()
               .HasOne(pi => pi.SanPham)
               .WithMany(p => p.Images)
               .HasForeignKey(pi => pi.SanPhamId);
            modelBuilder.Entity<SanPham>()
                .HasOne(sp => sp.NhaCungCap)
                .WithMany()
                .HasForeignKey(sp => sp.NhaCungCapId);

            modelBuilder.Entity<SmtpEmailSenderOptions>().HasNoKey();

            base.OnModelCreating(modelBuilder);
        }

    }
}

