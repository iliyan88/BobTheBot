using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BobTheBot
{
    public interface IUnitOfWork : RJ.Repository.Abstractions.IUnitOfWork
    {
        ISearchKeyRepository SearchKeyRepository { get; }
    }
}
