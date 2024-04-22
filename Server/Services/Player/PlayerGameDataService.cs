using Core.Data.Saves;
using SpaceServer.Database.Game;

namespace SpaceServer.Services.Player
{
    internal class PlayerGameDataService(GameSavesContext context) : GenericDbService<GameSavesContext, ulong, PlayerGameData>(context)
	{

	}
}
