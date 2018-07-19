using Hubtel.PosProxyData.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hubtel.PosProxyData.Repositories
{
    public class BaseRepository<TObject> where TObject : class
    {
        protected ApplicationDbContext _context;

        public ApplicationDbContext GetRawContext()
        {
            return _context;
        }

        public DbSet<TObject> GetContext()
        {
            return _context.Set<TObject>();
        }

        public async Task<int> ExecuteSqlCommandAsync(RawSqlString sql, CancellationToken token = default(CancellationToken))
        {
            return await _context.Database.ExecuteSqlCommandAsync(sql, token);
        }

        /// <summary>
        /// The contructor requires an open AppDataContext to work with
        /// </summary>
        /// <param name="context">An open AppDataContext</param>
        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _context.ChangeTracker.DetectChanges();
        }


        /// <summary>
        /// Returns a single object with a primary key of the provided id
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="id">The primary key of the object to fetch</param>
        /// <returns>A single object with the provided primary key or null</returns>
        public TObject Get(int id)
        {
            return _context.Set<TObject>().Find(id);
        }
        /// <summary>
        /// Returns a single object with a primary key of the provided id
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="id">The primary key of the object to fetch</param>
        /// <returns>A single object with the provided primary key or null</returns>
        public async Task<TObject> GetAsync(int id)
        {
            return await _context.Set<TObject>().FindAsync(id);
        }
        /// <summary>
        /// Gets a collection of all objects in the database
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <returns>An ICollection of every object in the database</returns>
        public ICollection<TObject> GetAll()
        {
            return _context.Set<TObject>().ToList();
        }
        /// <summary>
        /// Gets a collection of all objects in the database
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <returns>An ICollection of every object in the database</returns>
        public async Task<ICollection<TObject>> GetAllAsync()
        {
            return await _context.Set<TObject>().ToListAsync();
        }
        /// <summary>
        /// Returns a single object which matches the provided expression
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="match">A Linq expression filter to find a single result</param>
        /// <returns>A single object which matches the expression filter. 
        /// If more than one object is found or if zero are found, null is returned</returns>
        public TObject Find(Expression<Func<TObject, bool>> match)
        {
            return _context.Set<TObject>().SingleOrDefault(match);
        }
        /// <summary>
        /// Returns a single object which matches the provided expression
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="match">A Linq expression filter to find a single result</param>
        /// <returns>A single object which matches the expression filter. 
        /// If more than one object is found or if zero are found, null is returned</returns>
        public async Task<TObject> FindAsync(Expression<Func<TObject, bool>> match)
        {
            return await _context.Set<TObject>().SingleOrDefaultAsync(match);
        }
        /// <summary>
        /// Returns a collection of objects which match the provided expression
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="match">A linq expression filter to find one or more results</param>
        /// <returns>An ICollection of object which match the expression filter</returns>
        public ICollection<TObject> FindAll(Expression<Func<TObject, bool>> match)
        {
            return _context.Set<TObject>().Where(match).ToList();
        }
        /// <summary>
        /// Returns a collection of objects which match the provided expression
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="match">A linq expression filter to find one or more results</param>
        /// <returns>An ICollection of object which match the expression filter</returns>
        public async Task<ICollection<TObject>> FindAllAsync(Expression<Func<TObject, bool>> match)
        {
            return await _context.Set<TObject>().Where(match).ToListAsync();
        }
        /// <summary>
        /// Inserts a single object to the database and commits the change
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="t">The object to insert</param>
        /// <returns>The resulting object including its primary key after the insert</returns>
        public TObject Add(TObject t)
        {
            _context.Set<TObject>().Add(t);
            _context.SaveChanges();
            return t;
        }
        /// <summary>
        /// Inserts a single object to the database and commits the change
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="t">The object to insert</param>
        /// <returns>The resulting object including its primary key after the insert</returns>
        public async Task<TObject> AddAsync(TObject t)
        {
            _context.Set<TObject>().Add(t);
            await _context.SaveChangesAsync();
            return t;
        }
        /// <summary>
        /// Inserts a collection of objects into the database and commits the changes
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="tList">An IEnumerable list of objects to insert</param>
        /// <returns>The IEnumerable resulting list of inserted objects including the primary keys</returns>
        public IEnumerable<TObject> AddAll(IEnumerable<TObject> tList)
        {
            _context.Set<TObject>().AddRange(tList);
            _context.SaveChanges();
            return tList;
        }
        /// <summary>
        /// Inserts a collection of objects into the database and commits the changes
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="tList">An IEnumerable list of objects to insert</param>
        /// <returns>The IEnumerable resulting list of inserted objects including the primary keys</returns>
        public async Task<IEnumerable<TObject>> AddAllAsync(IEnumerable<TObject> tList)
        {
            _context.Set<TObject>().AddRange(tList);
            await _context.SaveChangesAsync();
            return tList;
        }
        /// <summary>
        /// Updates a single object based on the provided primary key and commits the change
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="updated">The updated object to apply to the database</param>
        /// <param name="key">The primary key of the object to update</param>
        /// <returns>The resulting updated object</returns>
        public TObject Update(TObject updated, int key)
        {
            if (updated == null)
                return null;

            TObject existing = _context.Set<TObject>().Find(key);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(updated);
                _context.SaveChanges();
            }
            return existing;
        }
        /// <summary>
        /// Updates a single object based on the provided primary key and commits the change
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="updated">The updated object to apply to the database</param>
        /// <param name="key">The primary key of the object to update</param>
        /// <returns>The resulting updated object</returns>
        public async Task<TObject> UpdateAsync(TObject updated, int key)
        {
            if (updated == null)
                return null;

            TObject existing = await _context.Set<TObject>().FindAsync(key);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(updated);
                await _context.SaveChangesAsync();
            }
            return existing;
        }
        /// <summary>
        /// Deletes a single object from the database and commits the change
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="t">The object to delete</param>
        public void Delete(TObject t)
        {
            _context.Set<TObject>().Remove(t);
            _context.SaveChanges();
        }
        /// <summary>
        /// Deletes a single object from the database and commits the change
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="t">The object to delete</param>
        public async Task<int> DeleteAsync(TObject t)
        {
            _context.Set<TObject>().Remove(t);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes multiple objects from the database and commits the change
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="tList">An IEnumerable list of objects to delete</param>
        public void DeleteRange(IEnumerable<TObject> tList)
        {
            _context.Set<TObject>().RemoveRange(tList);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets the count of the number of objects in the databse
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <returns>The count of the number of objects</returns>
        public int Count()
        {
            return _context.Set<TObject>().Count();
        }
        /// <summary>
        /// Gets the count of the number of objects in the databse
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <returns>The count of the number of objects</returns>
        public async Task<int> CountAsync()
        {
            return await _context.Set<TObject>().CountAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<ICollection<TObject>> TakeAsync(Expression<Func<TObject, bool>> match, int limit)
        {
            return await _context.Set<TObject>().Where(match).Take(limit).ToListAsync();
        }

        public ICollection<TObject> Take(Expression<Func<TObject, bool>> match, int limit)
        {
            return _context.Set<TObject>().Where(match).Take(limit).ToList();
        }

        public IQueryable<TObject> Where(Expression<Func<TObject, bool>> match)
        {
            return _context.Set<TObject>().Where(match);
        }

    }

    public interface IBaseRepository<TObject> : IDisposable where TObject : class
    {
        ApplicationDbContext GetRawContext();
        DbSet<TObject> GetContext();
        TObject Get(int id);
        Task<TObject> GetAsync(int id);
        ICollection<TObject> GetAll();
        Task<ICollection<TObject>> GetAllAsync();
        TObject Find(Expression<Func<TObject, bool>> match);
        ICollection<TObject> FindAll(Expression<Func<TObject, bool>> match);
        Task<ICollection<TObject>> FindAllAsync(Expression<Func<TObject, bool>> match);
        TObject Add(TObject t);
        Task<TObject> AddAsync(TObject t);
        IEnumerable<TObject> AddAll(IEnumerable<TObject> tList);
        Task<IEnumerable<TObject>> AddAllAsync(IEnumerable<TObject> tList);
        TObject Update(TObject updated, int key);
        Task<TObject> UpdateAsync(TObject updated, int key);
        void Delete(TObject t);
        Task<int> DeleteAsync(TObject t);
        void DeleteRange(IEnumerable<TObject> tList);
        int Count();
        Task<int> CountAsync();
        Task<ICollection<TObject>> TakeAsync(Expression<Func<TObject, bool>> match, int limit);
        ICollection<TObject> Take(Expression<Func<TObject, bool>> match, int limit);
        IQueryable<TObject> Where(Expression<Func<TObject, bool>> match);
        Task<int> ExecuteSqlCommandAsync(RawSqlString sql, CancellationToken token = default(CancellationToken));
    }
}
