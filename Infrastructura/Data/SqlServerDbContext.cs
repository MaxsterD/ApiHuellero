using Microsoft.EntityFrameworkCore;

namespace ApiConsola.Infrastructura.Data
{
    public partial class SqlServerDbContext: DbContext, ISqlServerDbContext
    {
        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : base(options) { }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
