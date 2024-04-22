using Microsoft.EntityFrameworkCore;
using Core.Data.Discord;
using Core.Game.Components;
using SpaceServer.Database.Base;

namespace SpaceServer.Database.Discord
{

    [InitContext]
    public class DiscordDataContext : ContextBase
    {
        public DiscordDataContext() : base("discord_data_context.db")
        {

        }

        public DbSet<CompanyDiscordData> CompanyDiscordData { get; protected set; }
        public DbSet<PlayerDiscordData> PlayerDiscordData { get; protected set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            RegisterValueConverters(typeof(Resource), configurationBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            AsSqliteContext(options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
