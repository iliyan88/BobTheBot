using Microsoft.EntityFrameworkCore;
using RJ.Repository.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BobTheBot
{
    public class SearchKeyRepository : SqlServerRepository<SearchKey>, ISearchKeyRepository
    {
        private readonly AppDbContext dbContext;

        public SearchKeyRepository(AppDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IReadOnlyList<SearchKey>> GetAllWords()
        {
            return await dbContext.SearchKeys.ToListAsync();
        }
        public async Task<SearchKey> GetWordById(int id)
        {
            return await dbContext.SearchKeys.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
