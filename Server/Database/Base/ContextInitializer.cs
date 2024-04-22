using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Core.Data.Saves;
using Core.Game.Components;
using SpaceServer.Services.Game.Generation;
using SpaceServer.Settings.Contexts;
using System.Reflection;

namespace SpaceServer.Database.Base
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class InitContextAttribute : Attribute
    {

    }

    internal class ContextInitializer
    {
        private ContextInitializerSettings Settings { get; }

        private IServiceProvider Services { get; }

        public ContextInitializer(IOptions<ContextInitializerSettings> settings, IServiceProvider services)
        {
            Settings = settings.Value;
            Services = services;

            FetchContexts();
        }

        private void FetchContexts()
        {
            var types = GetType().Assembly.GetTypes().Where(t =>
                t.GetCustomAttribute<InitContextAttribute>() is not null &&
                t.IsSubclassOf(typeof(ContextBase))
            );

            foreach (var type in types)
            {
                var context = (ContextBase)Services.GetRequiredService(type);

                string ensureDeletedIdentifier = context.GetType().Name + ".EnsureDeleted";
                string ensureCreatedIdentifier = context.GetType().Name + ".EnsureCreated";
                string initializeIdentifier = context.GetType().Name + ".Initialize";

                bool ensureDeleted = Settings.EnsureAllDeleted;
                bool ensureCreated = Settings.EnsureAllCreated;
                bool initialize = Settings.InitializeAll;

                if (Settings.Overrides.TryGetValue(ensureDeletedIdentifier, out object? tryDeleteString) &&
                    bool.TryParse(tryDeleteString as string, out var overrideDelete))
                {
                    ensureDeleted = overrideDelete;
                }

                if (Settings.Overrides.TryGetValue(ensureCreatedIdentifier, out object? tryCreatedString) &&
                    bool.TryParse(tryCreatedString as string, out var overrideCreate))
                {
                    ensureCreated = overrideCreate;
                }

                if (Settings.Overrides.TryGetValue(initializeIdentifier, out object? tryInitialize) &&
                    bool.TryParse(tryInitialize as string, out var overrideInitialize))
                {
                    initialize = overrideInitialize;
                }

                if (ensureDeleted) context.Database.EnsureDeleted();
                if (ensureCreated) context.Database.EnsureCreated();
                if (initialize) context.OnInitialize();
            }
        }
    }


}
