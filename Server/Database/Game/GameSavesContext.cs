using Microsoft.EntityFrameworkCore;
using SpaceCore.Data.Saves;
using SpaceCore.Game.Components;
using SpaceCore.Game.Entities.Buildables.Buildings;
using SpaceCore.Game.Space;
using SpaceCore.Game.Space.Bodies;
using SpaceServer.Database.Base;

namespace SpaceServer.Database.Game
{

    [InitContext]
    internal class GameSavesContext : ContextBase
    {

        public GameSavesContext() : base("game_saves_context.db")
        {

        }

        public DbSet<PlayerGameData> PlayerGameData { get; protected set; }

        //Cosmic Entities
        public DbSet<CosmicSystem> CosmicSystems { get; protected set; }
        public DbSet<Star> Stars { get; protected set; }
        public DbSet<Planet> Planets { get; protected set; }
        public DbSet<Moon> Moons { get; protected set; }

        //Buildings
        public DbSet<RecipeBuilding> RecipeBuildings { get; protected set; }


        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            RegisterValueConverters(typeof(Resource), configurationBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseLazyLoadingProxies();
            AsSqliteContext(options);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Buildable>()
            //	.HasOne(b => b.Parent as Region)
            //	.WithMany(r => r.Buildings);

            //modelBuilder.Entity<Region>()
            //	.HasOne(r => r.Parent as BaseBody)
            //	.WithMany(b => b.Regions);

            //modelBuilder.Entity<CosmicEntity>()

        }
    }
}
