namespace SpaceDiscordBot.Modules
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	internal class RegisterModuleAttribute(ModuleScope moduleScope) : Attribute
	{
		public ModuleScope ModuleScope { get; } = moduleScope;


	}
}
