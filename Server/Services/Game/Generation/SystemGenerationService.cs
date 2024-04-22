using Core.Game.Space;
using Core.Game.Space.Base;
using Core.Game.Space.Bodies;
using Core.Types;
using SpaceServer.Services.Utilization;

namespace SpaceServer.Services.Game.Generation
{
	internal class SystemGenerationService
	{
		private ResourceService ResourceService { get; }
		private NamingService NamingService { get; }

		public SystemGenerationService(ResourceService resourceService, NamingService namingService)
		{
			ResourceService = resourceService;
			NamingService = namingService;
		}

		public CosmicSystem GenerateHomeSystem(int seed)
		{
			Random random = new(seed);

			Star sol = new(100)
			{
				Name = NamingService.RandomName()
			};

			CosmicSystem homeSystem = new(sol);
			homeSystem.State = SystemState.Assimilated;

			//build rocky planets
			for (int i = 0; i < 4; i++)
			{
				Planet planet = Planet.AsRocky();

				//Details for home planet
				if (i == 3)
				{
					planet.Alias = planet.Name = "Capital";
					planet.WithTag(CosmicTag.Capital);

					for (int j = 0; j < 2; j++)
					{
						var moon = new Moon(1)
						{
							Name = NamingService.RandomName()
						};
						planet.WithMoon(moon);
					}
						
				} else
				{
					planet.Name = NamingService.RandomName();
				}

				//foreach (Region r in planet.Regions)
				//{
				//	int count = random.Next(1, 5);
				//	r.HarvestableResources.Add("iron_ore", (ExpoNumber)"69e");
				//}

				homeSystem.WithOrbiter(planet);
			}

			//build gas giants
			for (int i = 0; i < 2; i++)
			{
				Planet giant = Planet.AsGasGiant();
				giant.Name = NamingService.RandomName();

				homeSystem.WithOrbiter(giant);
			}

			//build ice giants
			for (int i = 0; i < 2; i++)
			{
				Planet iceGiant = Planet.AsIceGiant();
				iceGiant.Name = NamingService.RandomName();

				homeSystem.WithOrbiter(iceGiant);
			}

			return homeSystem;
		}

	}
}
