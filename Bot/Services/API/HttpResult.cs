using Discord;
using SpaceDiscordBot.Frameworks.Exceptions;
using System.Net;

namespace SpaceDiscordBot.Services.API
{
	internal class HttpResult<T>
	{
		public static HttpResult<TObject> Of<TObject>(TObject obj)
			=> new(HttpStatusCode.NoContent, true, obj);


		public HttpStatusCode Code { get; }

		public bool Success { get; }

		public T? Value { get; }

		public HttpResult(HttpStatusCode code, bool success, T? value)
		{
			Code = code;
			Success = success;
			Value = value;
		}


		public void EnsureSuccess(string source, LogSeverity severity = LogSeverity.Warning, params string[] message)
		{
			if (!Success)
			{
				var list = message.ToList();
				list.Add("Status Code: " + Code);

				throw new EmbedException(severity, source, list.ToArray());
			}
		}

		public T EnsureValue(string source, LogSeverity severity = LogSeverity.Warning, params string[] message)
		{
			EnsureSuccess(source, severity, message);
			if (Value is null) 
				throw new EmbedException(severity, source, "Ensure Object constraint failed");

			return Value;
		}
	}
}
