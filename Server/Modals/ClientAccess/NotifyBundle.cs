namespace SpaceServer.Modals.ClientAccess
{

	public enum NotificationType
	{
		Information, GameUpdate, GameAnnounce, GlobalAnnounce
	}

	public class NotifyBundle
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
