using Newtonsoft.Json.Serialization;

namespace SpaceServer.Settings.Binder
{
	class EntityFrameworkSerializationBinder : ISerializationBinder
	{
		private ISerializationBinder DefaultBinder { get; } = new DefaultSerializationBinder();

		public void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
		{
			assemblyName = null;

			if (serializedType.Namespace == "Castle.Proxies")
				typeName = serializedType?.BaseType?.FullName + ", SpaceCore";
			else
				//typeName = serializedType?.FullName + ", System.Private.CoreLib";
				DefaultBinder.BindToName(serializedType, out assemblyName, out typeName);
		}

		public Type BindToType(string? assemblyName, string typeName)
		{
			return DefaultBinder.BindToType(assemblyName, typeName);
		}
	}
}
