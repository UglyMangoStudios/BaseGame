using Core.Game.Components;
using SpaceDiscordBot.Services.API.Discord;

namespace SpaceDiscordBot.Services.Discord
{
	internal class ActionService(ChannelService channelService)
	{
		private static Func<ActionPacket> CreateActionPacket = null!;

		static ActionService()
		{
			ActionPacket.DoNothingWellMaybeSomethingButLikeNothingAtAll();
		}



		public ChannelService ChannelService { get; } = channelService;



		//For each player ID, give a corresponding Action packet
		private Dictionary<ulong, ActionPacket> PlayerActions { get; } = [];

		public ActionPacket GetPlayerPacket(ulong playerId)
		{
			if (PlayerActions.ContainsKey(playerId))
			{
				return PlayerActions[playerId];
			}

			ActionPacket packet = CreateActionPacket();
			PlayerActions.Add(playerId, packet);
			return packet;
		}


		//Each packet represents a focusing element within each channel id
		public class ActionPacket
		{
			static ActionPacket()
			{
				CreateActionPacket = () => new ActionPacket();
			}

			public static void DoNothingWellMaybeSomethingButLikeNothingAtAll() { }


			private Dictionary<ulong, IFocusable> ChannelFoci { get; } = [];

			private ActionPacket() { }

			public IFocusable? GetChannelFocus(ulong channelId) => ChannelFoci.TryGetValue(channelId, out var focus) ? focus : null;

			public bool SetChannelFocus(ulong channelId, IFocusable focus)
			{
				bool overwriting = ChannelFoci.ContainsKey(channelId);
				ChannelFoci[channelId] = focus;

				return overwriting;
			}


			public TFocus? GetFocusAs<TFocus>(ulong channelId) where TFocus : class, IFocusable => GetChannelFocus(channelId) as TFocus;

		}

	}
}
