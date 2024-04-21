namespace SpaceDiscordBot.http.Modals
{

	internal enum NotificationType
	{
		Information, GameUpdate, GameAnnounce, GlobalAnnounce
	}

	internal class NotifyBundle
	{
		public ulong Channel { get; }

		public NotificationType NotificationType { get; }

		public string MessageTitle { get; }

		public string MessageBody { get; }

		public NotifyBundle(ulong channel, NotificationType notificationType, string messageTitle, string messageBody)
		{
			Channel = channel;
			NotificationType = notificationType;
			MessageTitle = messageTitle;
			MessageBody = messageBody;
		}
	}
}
