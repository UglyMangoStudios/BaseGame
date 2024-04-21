namespace SpaceDiscordBot.Frameworks.Attributes
{

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	internal class RegisterListenerAttribute : Attribute
	{

		public string? Delimiter { get; }

		public RegisterListenerAttribute(string? delimiter = null)
		{
			Delimiter = delimiter;
		}
	}
}
