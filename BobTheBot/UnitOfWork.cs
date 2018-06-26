using RJ.Repository.SqlServer;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace BobTheBot
{
    public class UnitOfWork : SqlServerCommandUnitOfWork, IUnitOfWork
    {

        private readonly AppDbContext dbContext;
        private readonly Lazy<ISearchKeyRepository> searchKeyRepository;

        public ISearchKeyRepository SearchKeyRepository { get => searchKeyRepository.Value; }

        public UnitOfWork(AppDbContext dbContext, IServiceProvider serviceProvider) : base(dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            searchKeyRepository = new Lazy<ISearchKeyRepository>(() => serviceProvider.GetService<ISearchKeyRepository>());
        }
    }
}
