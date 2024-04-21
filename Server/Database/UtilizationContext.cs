using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog.Events;
using SpaceCore.Utility;
using SpaceServer.Database.Base;
using SpaceServer.Settings.Contexts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceServer.Database
{

    [InitContext]
	public class UtilizationContext : ContextBase
	{
		private UtilizationDataSettings UtilSettings { get; }

		public UtilizationContext(IOptions<UtilizationDataSettings> utilSettings) : base("util_context.db")
		{
			UtilSettings = utilSettings.Value;
		}


		private void ReadNamesFile()
		{
			var lines = FileUtility.ReadAllLines(UtilSettings.DefaultNamesFile);

			if (lines.Length == 0) return;

			Logger.Information("Default names file found. Reading and inserting any new names...");

			int count = 0;
			foreach (var line in lines)
			{
				if (line.StartsWith('#')) continue;

				string format = line.Trim(' ', '\t', '\r', '\n');
				if (line.Length != 0 && !EntryExists<NamingBundle>(n => n.Name == line))
				{
					NamingBundle bundle = new(line);
					Names.Add(bundle);

					count++;
				}
			}

			if (count == 0)
				Logger.Information("No new names exist. No changes have been made.");
			else
			{
				Logger.Information("Inserted {x} new names into the database.", count);
				base.SaveChanges();
			}
		}


		public DbSet<NamingBundle> Names { get; protected set; }

		protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
		{
			
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
			ReadNamesFile();
		}
	}


	[Table("names")]
	public class NamingBundle(string name)
	{

		[Column("name"), Key]
		public virtual string Name { get; private set; } = name;


		public override string ToString() => Name;
		public static implicit operator string(NamingBundle b) => b.Name;


		//Dangerous. Used for EF
		private NamingBundle() : this("unnamed") { }
	}

	[Table("logs")]
	public class LogEntry
	{
		public LogEntry(LogEvent logEvent)
		{
			
		}
	}


}
