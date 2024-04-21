using Serilog.Core;
using Serilog.Events;
using SpaceServer.Database;

namespace SpaceServer.Services.Utilization
{

	internal class LoggingService : ILogEventSink
	{
		public LoggingService()
		{

		}

		public void Emit(LogEvent logEvent)
		{
			OnSink?.Invoke(logEvent);
		}

		public event Action<LogEvent>? OnSink;

	}
}
