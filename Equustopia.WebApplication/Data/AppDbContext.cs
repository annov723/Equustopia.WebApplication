namespace Equustopia.WebApplication.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<Horse> Horses { get; set; }
        public DbSet<EquestrianCentre> EquestrianCentres { get; set; }
        public DbSet<UserData> UsersData { get; set; }
        
        public DbSet<pageViews> PagesViews { get; set; }
        public DbSet<mostViewedPages> MostViewedPages { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<UserData>().ToTable("userData", "main");

            modelBuilder.Entity<UserData>().HasMany(u => u.EquestrianCentres).WithOne(o => o.UserData)
                .HasForeignKey(o => o.userId).OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<UserData>().HasMany(u => u.Horses).WithOne(k => k.UserData)
                .HasForeignKey(k => k.userId).OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<UserData>().HasMany(u => u.pagesViews).WithOne(ws => ws.UserData)
                .HasForeignKey(ws => ws.userId).OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<EquestrianCentre>().HasOne(o => o.UserData).WithMany(u => u.EquestrianCentres)
                .HasForeignKey(o => o.userId).OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<EquestrianCentre>().HasMany(o => o.Horses).WithOne(u => u.EquestrianCentre)
                .HasForeignKey(o => o.centreId).OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<Horse>().HasOne(k => k.UserData).WithMany(u => u.Horses)
                .HasForeignKey(k => k.userId).OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Horse>().HasOne(k => k.EquestrianCentre).WithMany(o => o.Horses)
                .HasForeignKey(k => k.centreId).OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<pageViews>().HasOne(ws => ws.UserData).WithMany(u => u.pagesViews)
                .HasForeignKey(ws => ws.userId).OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<mostViewedPages>().HasNoKey().ToView("mostViewedPages", "analytics");
        }
    }

}