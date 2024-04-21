namespace SpaceDiscordBot.Frameworks.Attributes
{
    /// <summary>
    /// Simple attribute that, at runtime, goes through all of the slash commands and 
    /// registers them without the need to use <c>new SlashCommandBase();</c>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal class RegisterCommandAttribute : Attribute
    {
        /// <summary>
        /// If the this class should be ignored at runtime. Used if you want to prevent a slash command from becoming registered
        /// </summary>
        public bool Register { get; }

        /// <summary>
        /// Simple attribute that, at runtime, goes through all of the slash commands and 
        /// registers them without the need to use <c>new SlashCommandBase();</c>
        /// </summary>
        /// <param name="register">If true, this attribute will allow the slash command service to register this command
        ///  Default is <c>false</c></param>
        public RegisterCommandAttribute(bool register = true)
        {
            Register = register;
        }
    }
}
