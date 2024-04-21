namespace SpaceServer.Modals.Database
{
	/// <summary>
	/// The data representation of MongoDB settings used for this project
	/// </summary>
	internal class DatabaseSettings
	{
		public string ConnectionString { get; set; } = null!;
		public string Username {  get; set; } = null!;
		public string Password { get; set; } = null!;


		public string GameDatabase { get; set; } = null!;

		public string CompanyDataCollection { get; set; } = null!;	
		public string PlayerDataCollection { get; set; } = null!;
	}
}
