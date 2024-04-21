using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using SpaceCore.Extensions;
using SpaceDiscordBot.Settings;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace SpaceDiscordBot.Services.API
{
	internal class HttpService
	{ 
		private HttpClient client { get; }

		private ApiSettings _settings { get; }


		private JsonSerializerSettings DefaultSettings { get; } = new()
		{
			TypeNameHandling = TypeNameHandling.All,
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			PreserveReferencesHandling = PreserveReferencesHandling.All,
		};


		public HttpService(IOptions<ApiSettings> apiSettingsOptions)
		{
			client = new();
			_settings = apiSettingsOptions.Value;


			//Configure the api client
			client.BaseAddress = new Uri(_settings.Route);
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
		}

		protected StringContent JsonContent<T>(T data, JsonSerializerSettings? settings = null)
		{
			settings ??= DefaultSettings;
			string json = JsonConvert.SerializeObject(data, settings);
			return new StringContent(json, Encoding.UTF8, "application/json");
		}

		public string AppendPath(string root, params object?[] extensions) =>
			root + '/' + string.Join('/', extensions);


		//Keeping async Just In Case (TM)
		protected async Task<HttpResult<T>> CreateResult<T>(HttpResponseMessage response, JsonSerializerSettings? settings = null)
		{
			settings ??= DefaultSettings;

			if (!response.IsSuccessStatusCode)
			{
				Log.Debug("{Method}: Not successful status code. Code: {code}. Reason: {reason}", nameof(CreateResult), response.StatusCode, response.ReasonPhrase);
				return new HttpResult<T>(response.StatusCode, false, default);
			}

			if (response.StatusCode == HttpStatusCode.NoContent)
			{
				Log.Debug("{Method}: Success with no content", nameof(CreateResult));
				return new HttpResult<T>(response.StatusCode, true, default);
			}

			var stream = response.Content.ReadAsStream();

			T? deserialized = default;
			try
			{
				deserialized = JsonUtility.StreamDeserialize<T>(stream);
			}
			catch (Exception e)
			{
				Log.Fatal(e, nameof(CreateResult));
			}

			Log.Debug("{Method}: Success with JSON created object. Returning result.", nameof(CreateResult));

			stream.Flush();
			response.Dispose();

			return new HttpResult<T>(response.StatusCode, deserialized != null, deserialized);
		}



		protected async Task<HttpResult<TResult>> CreateResult<TResult>(Task<HttpResponseMessage> response, JsonSerializerSettings? settings = null)
			=> await CreateResult<TResult>(await response);



		public async Task<HttpResult<TResult>> GetAsync<TResult>(string path, JsonSerializerSettings? settings = null) =>
			await CreateResult<TResult>(client.GetAsync(path), settings);

		public async Task<HttpResult<TResult>> PostAsync<TData, TResult>(string path, TData data, JsonSerializerSettings? settings = null)
		{
			var content = JsonContent<TData>(data, settings);
			return await PostContentAsync<TResult>(path, content);
		}

		public async Task<HttpResult<TResult>> PostAsync<TResult>(string path, JsonSerializerSettings? settings = null)
		{
			return await PostContentAsync<TResult>(path, null);
		}

		public async Task<HttpResult<TResult>> PostContentAsync<TResult>(string path, HttpContent? content, JsonSerializerSettings? settings = null) =>
			await CreateResult<TResult>(client.PostAsync(path, content), settings);



		public async Task<HttpResult<TResult>> PutAsync<TData, TResult>(string path, TData data, JsonSerializerSettings? settings = null)
		{
			var content = JsonContent<TData>(data, settings);
			return await PutContentAsync<TResult>(path, content);
		}

		public async Task<HttpResult<TResult>> PutContentAsync<TResult>(string path, HttpContent content, JsonSerializerSettings? settings = null) =>
			await CreateResult<TResult>(client.PutAsync(path, content), settings);



		public async Task<HttpResult<TResult>> DeleteAsync<TResult>(string path, JsonSerializerSettings? settings = null) =>
			await CreateResult<TResult>(client.DeleteAsync(path), settings);


		


	}
}
