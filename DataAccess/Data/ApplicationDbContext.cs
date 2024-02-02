using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Models;
using Microsoft.AspNetCore.Identity;
using Utility;
using NetTopologySuite.Triangulate.Tri;


namespace DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BookTag> BookTags { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookBookTag> BookBookTags { get; set; }        
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<GenreBook> GenreBooks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<UserBookmark> UserBookmarks { get; set; }
        public DbSet<ChineseBook> ChineseBooks { get; set; }
        public DbSet<BookReading> BookReadings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Thêm cấu hình tùy chọn nếu cần thiết
            // Ví dụ: builder.Entity<ApplicationUser>().Property(u => u.SomeProperty).IsRequired();

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasMany(u => u.Books)
                .WithOne(b => b.ApplicationUser)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasMany(u => u.ChineseBooks)
                .WithOne(b => b.Book)
                .HasForeignKey(b => b.BookId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(b => b.Slug).IsUnique();
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasMany(u => u.Replies)
                .WithOne(b => b.ParentComment)
                .HasForeignKey(b => b.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            string userName = "lienminh9697@gmail.com";
            string password = "Thuong@123";

            // Thêm role vào cơ sở dữ liệu
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.HasData(
                    new IdentityRole { Id = "121a117a-b585-4172-b3cf-811ebce85dfc", Name = SD.ADMIN, NormalizedName = SD.ADMIN.ToUpper() },
                    new IdentityRole { Id = "16e81636-d2db-4303-b434-a8c1a79ccec6", Name = SD.MOD, NormalizedName = SD.MOD.ToUpper() },
                    new IdentityRole { Id = "174620a3-a7aa-4c3a-abb6-3a0b993d1a88", Name = SD.CONVERT, NormalizedName = SD.CONVERT.ToUpper() },
                    new IdentityRole { Id = "76fc64b4-2074-4828-9bd0-d2d30f99932d", Name = SD.AUTHOR, NormalizedName = SD.AUTHOR.ToUpper() },
                    new IdentityRole { Id = "dc7350ea-8bf2-43e8-a430-76b0766177b0", Name = SD.USER, NormalizedName = SD.USER.ToUpper() }
                // Thêm các role khác nếu cần
                );
            });

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasData(
                     new ApplicationUser
                     {
                         Id = "a2ea16da-c3fc-48fa-9a68-05dfe1623f7a",
                         UserName = "lienminh9697@gmail.com",
                         NormalizedUserName = "LIENMINH9697@GMAIL.COM",
                         Email = "lienminh9697@gmail.com",
                         NormalizedEmail = "LIENMINH9697@GMAIL.COM",
                         EmailConfirmed = true,
                         PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Thuong@123"),
                         SecurityStamp = string.Empty,
                     },
                     new ApplicationUser
                     {
                         Id = "a2ea16da-c3fc-48fa-9a68-0e1623f7a5df",
                         UserName = "khach.123@gmail.com",
                         NormalizedUserName = "KHAC.123@GMAIL.COM",
                         Email = "khac.123@gmail.com",
                         NormalizedEmail = "KHAC@123@GMAIL.COM",
                         EmailConfirmed = true,
                         PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Thuong@123"),
                         SecurityStamp = string.Empty,
                         FullName = "Khách Vãng Lai",
                     }
                 ); 
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.HasData(
                    new IdentityUserRole<string>
                    {
                        UserId = "a2ea16da-c3fc-48fa-9a68-05dfe1623f7a",
                        RoleId = "121a117a-b585-4172-b3cf-811ebce85dfc" // ID của vai trò USER
                    },
                     new IdentityUserRole<string>
                     {
                         UserId = "a2ea16da-c3fc-48fa-9a68-0e1623f7a5df",
                         RoleId = "dc7350ea-8bf2-43e8-a430-76b0766177b0" // ID của vai trò USER
                     }
                );
            });

            //言情  Ngôn tình   .
            //玄幻  Huyền huyễn .     
            //修真  Tu chân     . 
            //武侠  Võ hiệp     .     
            //穿越  Xuyên qua   .
            //都市  Đô thị      .
            //历史  Lịch sử     .
            //游戏  Trò chơi    .
            //科幻  Khoa huyễn  .
            //悬疑  Huyền nghi  .
            //同人  Đồng nhân   .
            //官场  Quan trường .
            //校园  Sân trường  .
            //网游  Võng du     .
            //灵异  Linh dị     .
            //仙侠  Tiên hiệp   .


            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasData(
                    new Genre { Id = 1, Name = "Ngôn Tình", ChineseName = "言情" },
                    new Genre { Id = 2, Name = "Huyền Huyễn", ChineseName = "玄幻" },
                    new Genre { Id = 3, Name = "Tu Chân", ChineseName = "修真" },
                    new Genre { Id = 4, Name = "Võ Hiệp", ChineseName = "武侠" },
                    new Genre { Id = 5, Name = "Xuyên Qua", ChineseName = "穿越" },
                    new Genre { Id = 6, Name = "Đô Thị", ChineseName = "都市" },
                    new Genre { Id = 7, Name = "Lịch Sử", ChineseName = "历史" },
                    new Genre { Id = 8, Name = "Trò Chơi", ChineseName = "游戏" },
                    new Genre { Id = 9, Name = "Khoa Huyễn", ChineseName = "科幻" },
                    new Genre { Id = 10, Name = "Huyền Nghi", ChineseName = "悬疑" },
                    new Genre { Id = 11, Name = "Đồng Nhân", ChineseName = "同人" },
                    new Genre { Id = 12, Name = "Quan Trường", ChineseName = "官场" },
                    new Genre { Id = 13, Name = "Sân Trường", ChineseName = "校园" },
                    new Genre { Id = 14, Name = "Võng Du", ChineseName = "网游" },
                    new Genre { Id = 15, Name = "Linh Dị", ChineseName = "灵异" },
                    new Genre { Id = 16, Name = "Tiên Hiệp", ChineseName = "仙侠" }
                );
            });

            modelBuilder.Entity<BookBookTag>()
            .HasKey(bbt => new { bbt.BookId, bbt.BookTagId });

            modelBuilder.Entity<GenreBook>()
            .HasKey(gb => new { gb.BookId, gb.GenreId });

            modelBuilder.Entity<UserBookmark>(entity =>
            {
                entity.HasKey(ubm => new { ubm.BookId, ubm.UserId });
            });

            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.ToTable(tb => tb.HasTrigger("UpdateBookUpdatedAt"));

                // entity.ToTable(tb => tb.HasTrigger("UpdateChapterIndex"));

                entity.HasIndex(c => c.ChineseBookId);

                // Đặt chỉ mục cho cả hai trường ChineseBookId và ChapterIndex
                entity.HasIndex(c => new { c.ChineseBookId, c.ChapterIndex });
            });
        }
    }
}
