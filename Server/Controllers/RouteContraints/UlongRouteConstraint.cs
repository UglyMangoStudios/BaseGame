using System.Globalization;

namespace SpaceServer.Controllers.RouteConstraints
{

	public class UlongRouteConstraint : IRouteConstraint
	{
		public static string UlongRouteConstraintName = "ulong";

		public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values,
			RouteDirection routeDirection)
		{
			if (routeKey == null)
				throw new ArgumentNullException(nameof(routeKey));

			if (values == null)
				throw new ArgumentNullException(nameof(values));

			if (!values.TryGetValue(routeKey, out var routeValue) || routeValue == null) return false;
			if (routeValue is ulong)
				return true;

			var valueString = Convert.ToString(routeValue, CultureInfo.InvariantCulture);
			return ulong.TryParse(valueString, out var _);
		}
	}
}
