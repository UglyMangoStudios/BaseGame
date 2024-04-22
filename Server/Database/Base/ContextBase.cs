using Microsoft.EntityFrameworkCore;
using Serilog;
using Core.Attributes;
using System.Reflection;
using ILogger = Serilog.ILogger;

namespace SpaceServer.Database.Base
{
    public abstract class ContextBase : DbContext
    {
        public string DB_PATH { get; }

        protected ILogger Logger { get; } = Serilog.Log.ForContext<ContextBase>();

        public ContextBase(string dbPath)
        {
            var folder = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "database");
            Directory.CreateDirectory(folder);
            DB_PATH = Path.Join(folder, dbPath);
        }

        protected abstract override void OnConfiguring(DbContextOptionsBuilder options);

        protected abstract override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder);

        protected abstract override void OnModelCreating(ModelBuilder modelBuilder);

        public virtual void OnInitialize() { }

        protected void AsSqliteContext(DbContextOptionsBuilder options)
        {
            Logger.Verbose("Building SQLite database source to path {path}", DB_PATH);
            options.UseSqlite($"Data Source={DB_PATH}");
        }

        /// <summary>
        /// Automatically registers every object in the same assembly as reference with their corresponding value converters. 
        /// This only works on objects with the attribute <see cref="RegisterValueConverterAttribute"/>.
        /// </summary>
        /// <param name="reference">The targeting assembly</param>
        /// <param name="configurationBuilder">The builder of the <see cref="DbContext"/> to utilize.</param>
        protected void RegisterValueConverters(Type reference, ModelConfigurationBuilder configurationBuilder)
        {
            foreach (Type type in reference.Assembly.GetTypes())
            {
                var attribute = type.GetCustomAttribute<RegisterValueConverterAttribute>();
                if (attribute is null) continue;

                configurationBuilder.Properties(type).HaveConversion(attribute.ValueConverter, attribute.ValueComparer);
            }
        }


        protected bool EntryExists<T>(Func<T, bool> predicate)
            where T : class
        {
            return ChangeTracker.Entries().
                Where(e => e.Entity is T t && predicate.Invoke(t))
                .Any()

                || Set<T>().Any(predicate);
        }


    }
}
