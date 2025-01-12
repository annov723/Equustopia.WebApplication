namespace Equustopia.WebApplication.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models.Analytics;
    using Models.Helpers;
    using Models.Main;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<Horse> Horses { get; set; }
        public DbSet<EquestrianCentre> EquestrianCentres { get; set; }
        public DbSet<UserData> UsersData { get; set; }
        
        public DbSet<PagesViews> PagesViews { get; set; }
        public DbSet<MostViewedPages> MostViewedPages { get; set; }
        
        public DbSet<PublicHorses> PublicHorses { get; set; }
        public DbSet<PublicUsers> PublicUsers { get; set; }
        public DbSet<ApprovedEquestrianCentres> ApprovedEquestrianCentres { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<UserData>().ToTable("userData", "main");

            modelBuilder.Entity<UserData>().HasMany(u => u.EquestrianCentres).WithOne(o => o.UserData)
                .HasForeignKey(o => o.userId).OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<UserData>().HasMany(u => u.Horses).WithOne(k => k.UserData)
                .HasForeignKey(k => k.userId).OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<UserData>().HasMany(u => u.PagesViews).WithOne(ws => ws.UserData)
                .HasForeignKey(ws => ws.userId).OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<EquestrianCentre>().HasOne(o => o.UserData).WithMany(u => u.EquestrianCentres)
                .HasForeignKey(o => o.userId).OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<EquestrianCentre>().HasMany(o => o.Horses).WithOne(u => u.EquestrianCentre)
                .HasForeignKey(o => o.centreId).OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<Horse>().HasOne(k => k.UserData).WithMany(u => u.Horses)
                .HasForeignKey(k => k.userId).OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Horse>().HasOne(k => k.EquestrianCentre).WithMany(o => o.Horses)
                .HasForeignKey(k => k.centreId).OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<PagesViews>().HasOne(ws => ws.UserData).WithMany(u => u.PagesViews)
                .HasForeignKey(ws => ws.userId).OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<MostViewedPages>().HasNoKey().ToView("mostViewedPages", "analytics");
            
            modelBuilder.Entity<PublicHorses>().HasNoKey().ToView("publicHorses", "main");
            modelBuilder.Entity<PublicUsers>().HasNoKey().ToView("publicUsers", "main");
            modelBuilder.Entity<ApprovedEquestrianCentres>().HasNoKey().ToView("approvedEquestrianCentres", "main");
            
            modelBuilder.Entity<HorseAgeGroup>().HasNoKey();
            modelBuilder.Entity<HorseBreedGroup>().HasNoKey();
            modelBuilder.Entity<CentreViewsByDate>().HasNoKey();
        }
    }

}