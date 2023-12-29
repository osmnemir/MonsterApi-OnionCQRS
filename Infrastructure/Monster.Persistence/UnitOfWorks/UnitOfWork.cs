using Monster.Application.Interfaces.Repositories;
using Monster.Application.Interfaces.UnitOfWorks;
using Monster.Persistence.Context;
using Monster.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monster.Persistence.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWorks
    {
        private readonly ApiDbContext dbContext;

        public UnitOfWork(ApiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async ValueTask DisposeAsync() => await dbContext.DisposeAsync();
        public int Save() => dbContext.SaveChanges();
        public async Task<int> SaveAsync() => await dbContext.SaveChangesAsync();
        IReadRepository<T> IUnitOfWorks.GetReadRepository<T>() => new ReadRepository<T>(dbContext);
        IWriteRepository<T> IUnitOfWorks.GetWriteRepository<T>() => new WriteRepository<T>(dbContext);

    }
}
