using Castle.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SpaceServer.Database.Base;
using ILogger = Serilog.ILogger;

namespace SpaceServer.Services
{
    public class GenericDbService<TContextBase, TId, TObject>
		where TContextBase : ContextBase 
		where TObject : class
	{
		protected TContextBase _context { get; }

		protected ILogger Logger { get; }


		public GenericDbService(TContextBase context)
		{
			_context = context;
			Logger = Serilog.Log.ForContext<TObject>();
		}


		protected async Task<List<TObject>> GetListAsync(DbSet<TObject> set)
		{
			Logger.Debug("{name} queued to retrieve all data points for set of type {type}", nameof(GetListAsync), set.EntityType.DisplayName());
			return await set.ToListAsync();
		}


		public async Task<TObject?> GetAsync(TId id)
		{
			Logger.Debug("{name} queued for object {object} id: {id}", nameof(GetAsync), typeof(TObject).Name, id);
			return await _context.FindAsync<TObject>(id);
		}


		public TObject? GetByProxy(Func<TObject, bool> filter)
		{
			return _context.Set<TObject>().AsNoTracking().Where(filter).FirstOrDefault();
		}


		private static T? Unwrap<T>(T? @object) where T : class
		{
			if (@object is not IProxyTargetAccessor proxy)
				return @object;

			return proxy?.DynProxyGetTarget() as T;
		}
			

		public virtual async Task CreateAsync(TObject @object)
		{
			Logger.Debug("{name} queued for new entry of type {object}", nameof(CreateAsync), typeof(TObject).Name);
			_context.Add(@object);
			await _context.SaveChangesAsync();
		}

		public virtual async Task UpdateAsync(TId id, TObject @object)
		{
			if (await GetAsync(id) is TObject found)
			{
				Logger.Debug("{name} detaching existing object {object} for id: {id}", nameof(UpdateAsync), typeof(TObject).Name, id);
				_context.Entry(found).State = EntityState.Detached;
			}

			Logger.Debug("{name} on object {object} with id: {id}", nameof(UpdateAsync), typeof(TObject).Name, id);
			_context.Update(@object);

			await _context.SaveChangesAsync();
		}

		public virtual async Task RemoveAsync(TObject @object)
		{
			Logger.Debug("{name} queued for object {object}", nameof(RemoveAsync), typeof(TObject).Name);

			_context.Remove(@object);
			await _context.SaveChangesAsync();
		}

	}
}
