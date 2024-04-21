using Microsoft.Extensions.ObjectPool;
using SpaceServer.Database;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SpaceServer.Services.Utilization
{
	public class NamingService(UtilizationContext utilizationContext)
	{
		private UtilizationContext UtilizationContext { get; } = utilizationContext;

		private Random Random { get; } = new();


		public bool NameExists(string name) =>
			UtilizationContext.Names.Any(s => s.Name == name);

		public string IndexName(int index) =>
			UtilizationContext.Names.ElementAt(index).ToString();

		public int CountNames() =>
			UtilizationContext.Names.Count();


		public string RandomName() =>
			IndexName(Random.Next(0, CountNames()));


		public string[] RandomNames(int count)
		{
			string[] names = new string[count];

			for (int i = 0; i < count; i++)
				names[i] = RandomName();

			return names;
		}

		public async Task AddName(params string[] names)
		{
			names.ToList().ForEach(name => UtilizationContext.Add(name));
			await UtilizationContext.SaveChangesAsync();
		}
			

		public string[] GetNames() =>
			UtilizationContext.Names.Select(n => n.ToString()).ToArray();


		public async Task RemoveName(string name)
		{
			var find = this.UtilizationContext.ChangeTracker.Entries()
				.Where(entry => entry.Entity is NamingBundle n && n.Name == name)
				.FirstOrDefault();

			if (find is null)
			{
				var maybe = UtilizationContext.Names.FirstOrDefault(n => n.Name == name);
				if (maybe is not null) UtilizationContext.Names.Remove(maybe);
			}
			else find.State = Microsoft.EntityFrameworkCore.EntityState.Deleted;

			await UtilizationContext.SaveChangesAsync();
		}
			

	}
}
