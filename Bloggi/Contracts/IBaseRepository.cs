using Bloggi.Consts;
using Bloggi.Helpers;
using Bloggi.ViewModels;
using System.Linq.Expressions;

namespace Bloggi.Contracts;

public interface BaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync(string[] includes=null, Expression<Func<T, object>> orderBy = null, string orderDirection = OrderByConst.Ascending);
    Task<T?> FindOneAsync(Expression<Func<T, bool>> expression, string[] includes = null);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression , string[] includes=null,
                    Expression<Func<T,object>> orderBy=null,string orderDirection=OrderByConst.Ascending);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T> CreateAsync(T entity);
    Task<int> CountAsync();
    //PageSize => for number of items will appears per page
    //PageNumber => for the Current Page
    //PageCount => for the number of pages available based on the number of the item list
    IEnumerable<T> ApplyPagination(IEnumerable<T> List,int PageNumber,int pageCount, int pageSize);

}
