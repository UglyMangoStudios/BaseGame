namespace SpaceDiscordBot.Modules
{

    /// <summary>
    /// The locations where a command are allowed to be registered. 
    /// </summary>
    internal enum ModuleScope
    {
        ///<summary> Behaves as global, but only gets added to the guilds the bot is connected to </summary>
        Guilds,
        ///<summary> Actually a global command. Use sparingly. Takes up to 1hr to update on Discord </summary>
        Global,
        ///<summary> Command is only available on the Headquarters Server </summary>
        HQ,
        ///<summary> Commands that are only available at the company level, regardless if they are founded </summary>
        Company,
        ///<summary> Command is only available at Company guilds before the company is officially established </summary>
        PreEstablishedCompany,
        ///<summary> Command is only available at Company guilds after the company has been officially established </summary>
        EstablishedCompany,
    }
}
