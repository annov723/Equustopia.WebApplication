namespace Equustopia.WebApplication.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<Kon> Konie { get; set; }
        public DbSet<Osrodek> Osrodki { get; set; }
        public DbSet<Uzytkownik> Uzytkownicy { get; set; }
        
        public DbSet<WyswietleniaStron> WyswietleniaStron { get; set; }
        public DbSet<NajczesciejWyswietlaneStrony> NajczesciejWyswietlaneStrony { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Uzytkownik>().HasMany(u => u.Osrodki).WithOne(o => o.Uzytkownik)
                .HasForeignKey(o => o.UzytkownikId).OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Uzytkownik>().HasMany(u => u.Konie).WithOne(k => k.Uzytkownik)
                .HasForeignKey(k => k.UzytkownikId).OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Uzytkownik>().HasMany(u => u.WyswietleniaStron).WithOne(ws => ws.Uzytkownik)
                .HasForeignKey(ws => ws.UzytkownikId).OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Osrodek>().HasOne(o => o.Uzytkownik).WithMany(u => u.Osrodki)
                .HasForeignKey(o => o.UzytkownikId).OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Osrodek>().HasMany(o => o.Konie).WithOne(u => u.Osrodek)
                .HasForeignKey(o => o.OsrodekId).OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<Kon>().HasOne(k => k.Uzytkownik).WithMany(u => u.Konie)
                .HasForeignKey(k => k.UzytkownikId).OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Kon>().HasOne(k => k.Osrodek).WithMany(o => o.Konie)
                .HasForeignKey(k => k.OsrodekId).OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<WyswietleniaStron>().HasOne(ws => ws.Uzytkownik).WithMany(u => u.WyswietleniaStron)
                .HasForeignKey(ws => ws.UzytkownikId).OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<NajczesciejWyswietlaneStrony>().HasNoKey().ToView("NajczesciejWyswietlaneStrony", "analytics");
        }
    }

}