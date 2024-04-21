namespace SpaceServer.Settings.Contexts
{
	public class ContextInitializerSettings
	{
		public bool EnsureAllCreated { get; set; } = true;
		public bool EnsureAllDeleted { get; set; } = false;
		public bool InitializeAll { get; set; } = true;

		public Dictionary<string, object> Overrides { get; set; } = [];

	}
}
