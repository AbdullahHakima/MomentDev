using Bloggi.Consts;
using Bloggi.Contracts;
using Bloggi.Data;
using Bloggi.Helpers;
using Bloggi.Models;
using Bloggi.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Bloggi.Repositories;

public class BaseRepository<T> : Contracts.BaseRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;

    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
    } 
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }
    public async Task<IEnumerable<T>> GetAllAsync(string[]includes=null, Expression<Func<T, object>> orderBy = null, string orderDirection = OrderByConst.Ascending)
    {
        IQueryable<T> query=_context.Set<T>();
        if (includes != null)
            foreach (var include in includes)
                query = query.Include(include);
        if(orderBy != null)
            if(orderDirection!=OrderByConst.Ascending)
                query = query.OrderByDescending(orderBy);

        return await query.ToListAsync();
    }

    public async Task<T> CreateAsync(T entity)
    {
          await _context.Set<T>().AddAsync(entity);
        return entity;
    }

    public Task DeleteAsync(T entity)
    {
       _context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        return entity;
    }
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression, string[] includes = null, 
        Expression<Func<T, object>> orderBy = null, string orderDirection = "ASC")
    {
        IQueryable<T> query = _context.Set<T>().Where(expression);
        if(includes != null)
            foreach(var include in includes)
                query = query.Include(include);
        if(orderBy!=null)
            if(orderDirection==OrderByConst.Ascending)
                query = query.OrderBy(orderBy);
            else
                query = query.OrderByDescending(orderBy);
        return await query.ToListAsync();
    }

    public async Task<T?> FindOneAsync(Expression<Func<T, bool>> expression, string[] includes = null)
    {
        IQueryable<T> query=_context.Set<T>();
        if(includes != null)
            foreach(var include in includes)
                query = query.Include(include);
        return await query.SingleOrDefaultAsync(expression);
    }

    public Task<int> CountAsync()
    {
        return _context.Set<T>().CountAsync();
    }

    public IEnumerable<T> ApplyPagination(IEnumerable<T> List, int PageNumber, int pageCount,int pageSize)
    {
        var Data = List;
        var paginate = new Pagination<T>(data: Data,pageCount, PageNumber,pageSize:pageSize);
        int itemToSkip = (paginate.PageIndex - 1) * paginate.PageSize;
        return Data.Skip(itemToSkip).Take(paginate.PageSize).ToList();
    }
    

}
