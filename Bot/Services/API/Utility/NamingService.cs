using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceDiscordBot.Services.API.Utility
{
	internal class NamingService(HttpService httpService) : 
		GenericApiObjectRetriever<string, string>(httpService, "util/naming")
	{
		protected override Func<string, string> ObjectIdPredicate => id => id;

		protected override bool UseCache => false;

		public new Task<HttpResult<List<string>>> GetAllAsync() => base.GetAllAsync();


		public async Task<HttpResult<List<string>>> GetRandomListAsync(int count = 1)
		{
			string path = HttpService.AppendPath(ApiPath, "random", count);
			return await HttpService.GetAsync<List<string>>(path);
		}

		public async Task<HttpResult<string>> GetRandomAsync()
		{
			string path = HttpService.AppendPath(ApiPath, "random");
			return await HttpService.GetAsync<string>(path);
		}
	}
}
