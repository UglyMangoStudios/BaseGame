using SwaggerThemes;

namespace SpaceServer.Settings
{
	public class SwaggerSettings
	{

		public string ThemeName { get; set; } = "XCodeLight";
		public string? CustomCSS { get; set; }

		public Theme GetTheme()
		{
			return typeof(Theme).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
				.FirstOrDefault(prop => prop.Name == ThemeName)?
				.GetValue(null) as Theme
				
				?? Theme.XCodeLight;
		}


	}
}
