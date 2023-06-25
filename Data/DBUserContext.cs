using Microsoft.EntityFrameworkCore;

namespace UserAPI.Data
{
    public partial class DBUserContext : DbContext
    {
        public DBUserContext()
        {
        }

        public DBUserContext(DbContextOptions<DBUserContext> options)
            : base(options)
        {
        }
        public virtual DbSet<User> Users { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=UserApi;Integrated Security=True;");
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {

                entity.Property(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.MarketingConsent).IsRequired().HasDefaultValue(false);
        
            });
        }
    }
}
