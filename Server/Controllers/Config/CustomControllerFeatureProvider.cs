


using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace SpaceServer.Controllers.Config
{

	//Provided by
	//https://stackoverflow.com/questions/41824957/can-i-make-an-asp-net-core-controller-internal
	internal class CustomControllerFeatureProvider : ControllerFeatureProvider
	{
		protected override bool IsController(TypeInfo typeInfo)
		{
			var isCustomController = !typeInfo.IsAbstract && typeof(InternalController).IsAssignableFrom(typeInfo);
			return isCustomController || base.IsController(typeInfo);
		}
	}
}
