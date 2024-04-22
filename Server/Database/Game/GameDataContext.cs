using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Core.Game.Components;
using Core.Game.Entities.Buildables.Buildings;
using Core.Utility;
using SpaceServer.Database.Base;
using SpaceServer.Settings.Contexts;

namespace SpaceServer.Database.Game
{

    [InitContext]
    public class GameDataContext : ContextBase
    {

        private GameDataSettings GameDataSettings { get; }

        public GameDataContext(IOptions<GameDataSettings> gameDataSettings) : base("game_data_context.db")
        {
            GameDataSettings = gameDataSettings.Value;
        }

        public DbSet<Resource> Resources { get; protected set; }
        public DbSet<Recipe> Recipes { get; protected set; }


        //Building factories
        public DbSet<RecipeBuildingFactory> RecipeBuildingFactories { get; protected set; }


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

        public override void OnInitialize()
        {
            FillData();
        }

        private void FillData()
        {
            Logger.Information("Filling game data from files...");
            FillResources();

            base.SaveChanges();
            Logger.Information("Game data insertion completed.");
        }

        private void FillResources()
        {
            string[] lines = FileUtility.ReadAllLines(GameDataSettings.ResourcesCsvPath);
            if (lines.Length == 0) return;

            Logger.Information("Inserting new resource data...");

            int count = 0;
            foreach (string line in lines.Skip(1))
            {
                if (line.StartsWith('#')) continue;

                string[] values = line.Trim(' ', '\n', '\r', '\t').Split(';');
                if (values.Length != 5) continue;

                Group group = Enum.Parse<Group>(values[0]);
                string id = values[1];
                string name = values[2];
                Rarity rarity = Enum.Parse<Rarity>(values[3]);
                string description = values[4];

                //TODO: Add url and emoji data

                if (EntryExists<Resource>(r => r.Id == id)) continue;

                Resource insert = new(id, name, rarity, group, description);
                base.Add(insert);
                count++;
            }

            if (count == 0)
                Logger.Information("No new resource data detected.");
            else
                Logger.Information("Successfully inserted and saved {c} resource(s)", count);
        }
    }
}
