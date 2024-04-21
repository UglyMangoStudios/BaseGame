using Serilog;
//using SpaceCore.Game.Space.Bodies.Components;

namespace SpaceDiscordBot.Services.API
{
	internal abstract class GenericApiObjectRetriever<TId, TObject>
		where TId : notnull
	{

		protected Dictionary<TId, TObject> CachedObjects { get; } = [];

		protected HttpService HttpService { get; }

		protected string ApiPath {  get; }

		protected ILogger Logger { get; }

		protected virtual bool UseCache { get; } = true;

		protected GenericApiObjectRetriever(HttpService httpService, string apiPath)
		{
			HttpService = httpService;
			ApiPath = apiPath;

			Logger = Log.ForContext<TObject>();
		}

		protected string BuildPath(TId id) => HttpService.AppendPath(ApiPath, id);


		protected virtual async Task<HttpResult<List<TObject>>> GetAllAsync()
		{
			Logger.Debug("{Method} to {path}", nameof(GetAllAsync), ApiPath);

			var result = await HttpService.GetAsync<List<TObject>>(ApiPath);

			if(UseCache && result.Success && result.Value is not null)
			{
				foreach(var item in result.Value)
					ToCache(ObjectIdPredicate.Invoke(item), item);
			}

			return result;
		}

		protected virtual async Task<HttpResult<TObject>> GetAsync(TId id)
		{
			string path = BuildPath(id);
			Logger.Debug("{Method} to {path}", nameof(GetAsync), path);

			if (UseCache && ValidateCache(id, out var obj)) 
				return HttpResult<TObject>.Of(obj!);

			return await HttpService.GetAsync<TObject>(path);
		}

		protected virtual async Task<HttpResult<TObject>> CreateAsync(TObject @object)
		{
			Logger.Debug("{Method} to {path}", nameof(CreateAsync), ApiPath);

			var response = await HttpService.PostAsync<TObject, TObject>(ApiPath, @object);

			if (UseCache) TryCacheResult(response);

			return response;
		}

		protected virtual async Task<HttpResult<TObject>> UpdateAsync(TId id, TObject @object)
		{
			string path = BuildPath(id);
			Logger.Debug("{Method} to {path}", nameof(UpdateAsync), path);

			var response = await HttpService.PutAsync<TObject, TObject>(path, @object);

			if (UseCache) TryCacheResult(response);

			return response;
		}

		protected virtual async Task<HttpResult<bool>> DeleteAsync(TId id)
		{
			string path = BuildPath(id);
			Logger.Debug("{Method} to {path}", nameof(DeleteAsync), path);

			var response = await HttpService.DeleteAsync<bool>(path);

			if (UseCache && response.Success && IsCached(id))
				CachedObjects.Remove(id);

			return response;
		}



		//Dealings with caching 
		protected abstract Func<TObject, TId> ObjectIdPredicate { get; }

		/// <summary>
		/// TODO: IMPLEMENT and switch to abstract
		/// <br/>
		/// Predicate that checks if an object is out of date. Will do some sort of header check
		/// </summary>
		/// <returns>The predicate</returns>
		protected virtual Func<TObject, bool> CheckOutdated() => obj => true;


		protected virtual TObject? Cached(TId id) => 
			CachedObjects.TryGetValue(id, out TObject? found) ? found : default;

		/// <summary>
		/// Adds or overwrites an object with the specified id to the cache
		/// </summary>
		/// <param name="id">The object's id</param>
		/// <param name="object">The object to cache</param>
		/// <returns>A boolean indicating success or failure</returns>
		protected virtual bool ToCache(TId id, TObject? @object)
		{
			if (@object is null) return false;
			CachedObjects[id] = @object;
			return true;
		}


		protected bool ToCache(TId id, HttpResult<TObject> response) =>
			ToCache(id, response.Value);


		protected virtual bool IsCached(TId id) => 
			CachedObjects.ContainsKey(id);
		

		protected virtual bool TryCacheResult(HttpResult<TObject> response)
		{
			if (response.Success && response.Value is not null)
			{
				var obj = response.Value!;
				ToCache(ObjectIdPredicate.Invoke(obj), obj);

				return true;
			}

			return false;
		}


		protected virtual bool ValidateCache(TId id, out TObject? obj)
		{
			var firstTry = Cached(id);

			if (firstTry is not null && CheckOutdated().Invoke(firstTry))
			{
				obj = firstTry;
				return true;
			}

			obj = default;
			return false;
		}
	}
}
