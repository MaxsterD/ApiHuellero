using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ApiConsola.Infrastructura.Data
{
    public interface ISqlServerDbContext
    {
        DatabaseFacade Database { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
        void Dispose();
    }
}
