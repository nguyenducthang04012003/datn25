using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using PharmaDistiPro.Models;

namespace PharmaDistiPro.Repositories.Infrastructures
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        T GetById(object id);
        Task<T> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetByConditionAsync(Expression<Func<T, bool>> expression, string[]? includes = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);

        Task<T> GetSingleByConditionAsync(Expression<Func<T, bool>> expression, string[]? includes = null);

        Task<T> InsertAsync(T obj);

        Task<T> UpdateAsync(T obj);

        Task<T> DeleteAsync(T entity);

        Task<IEnumerable<T>> GetPagedAsync(Expression<Func<T, bool>> filter, int pageNumber, int pageSize, string[]? includes = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
        Task<int> CountAsync(Expression<Func<T, bool>> expression);
        Task<int> SaveAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
       
    }

}