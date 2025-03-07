using BaseChord.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BaseChord.Application.Repositories;

public interface IGenericRepository<TEntity> where TEntity : IEntity
{
    Task<TEntity> AddAsync(TEntity entity);

    Task UpdateAsync(TEntity entity);

    Task RemoveAsync(TEntity entity);

    Task RemoveAsync(Guid id);

    Task<TEntity?> FindByEntityAsync(Guid id);
}
