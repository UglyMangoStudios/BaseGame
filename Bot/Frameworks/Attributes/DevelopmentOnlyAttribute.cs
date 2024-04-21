namespace SpaceDiscordBot.Frameworks.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	internal class LimitRuntimeAttribute(Runtime runtime): Attribute
	{
		public Runtime Runtime { get; } = runtime;
	}


	internal enum Runtime
	{
		Development, Production, None
	}
}
