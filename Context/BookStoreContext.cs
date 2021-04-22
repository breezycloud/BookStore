using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using BookStore.Models;

#nullable disable

namespace BookStore.Context
{
    public partial class BookStoreContext : DbContext
    {
        public BookStoreContext()
        {
        }

        public BookStoreContext(DbContextOptions<BookStoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=BookStoreContext");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Author)
                    .HasMaxLength(100)
                    .HasColumnName("author")
                    .HasDefaultValueSql("('N/A')");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .HasColumnName("description")
                    .HasDefaultValueSql("('N/A')");

                entity.Property(e => e.Genre)
                    .HasMaxLength(50)
                    .HasColumnName("genre")
                    .HasDefaultValueSql("('N/A')");

                entity.Property(e => e.Isbn)
                    .HasMaxLength(50)
                    .HasColumnName("isbn")
                    .HasDefaultValueSql("('N/A')");

                entity.Property(e => e.PublishedDate)
                    .HasColumnType("date")
                    .HasColumnName("publishedDate");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title")
                    .HasDefaultValueSql("('N/A')");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Bookid).HasColumnName("bookid");

                entity.Property(e => e.DateBooked)
                    .HasColumnType("date")
                    .HasColumnName("dateBooked");

                entity.Property(e => e.DateReturned)
                    .HasColumnType("date")
                    .HasColumnName("dateReturned");

                entity.Property(e => e.Status)
                    .HasMaxLength(30)
                    .HasColumnName("status")
                    .HasDefaultValueSql("('N/A')");

                entity.Property(e => e.Userid).HasColumnName("userid");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.Bookid)
                    .HasConstraintName("FK__Bookings__bookid__2F10007B");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.Userid)
                    .HasConstraintName("FK__Bookings__userid__300424B4");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Roleid)
                    .ValueGeneratedNever()
                    .HasColumnName("roleid");

                entity.Property(e => e.Roletype)
                    .HasMaxLength(20)
                    .HasColumnName("roletype");
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Available)
                    .HasColumnName("available")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Bookid).HasColumnName("bookid");

                entity.Property(e => e.Total)
                    .HasColumnName("total")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.Stocks)
                    .HasForeignKey(d => d.Bookid)
                    .HasConstraintName("FK__Stocks__bookid__33D4B598");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(100)
                    .HasColumnName("address");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("dob");

                entity.Property(e => e.Email)
                    .HasMaxLength(200)
                    .HasColumnName("email");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Passwordhash)
                    .IsRequired()
                    .HasColumnName("passwordhash");

                entity.Property(e => e.Roleid).HasColumnName("roleid");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.Roleid)
                    .HasConstraintName("FK__Users__roleid__25869641");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
