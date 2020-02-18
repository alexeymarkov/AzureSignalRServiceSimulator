namespace AzureSignalRServiceSimulator
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Reflection.Emit;

	public class HubFactory
	{
		private IDictionary<string, Type> _hubTypes = new Dictionary<string, Type>();

		public string[] HubNames => _hubTypes.Keys.ToArray();

		public Type this[string hubName] => _hubTypes[hubName.ToLower()];

		public void Build(string[] hubNames)
		{
			var assemblyName = string.Concat(GetType().Namespace, ".", Path.GetRandomFileName());
			var asmName = new AssemblyName(assemblyName);
			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);

			var ctorArguments = new[] { typeof(IServiceProvider), typeof(HubFactory) };
			var baseType = typeof(BaseHub);
			var baseConstructor = baseType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance, null, ctorArguments, null);


			foreach (var hubName in hubNames.Select(x => x.ToLower()))
			{
				var typeBuilder = moduleBuilder.DefineType(hubName, TypeAttributes.Public, typeof(BaseHub));

				var constructoreBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, ctorArguments);

				var ilGenerator = constructoreBuilder.GetILGenerator();
				ilGenerator.Emit(OpCodes.Ldarg_0);                  // push this onto stack
				ilGenerator.Emit(OpCodes.Ldarg_1);                  // push this 1st parameter onto stack
				ilGenerator.Emit(OpCodes.Ldarg_2);                  // push this 2nd parameter onto stack
				ilGenerator.Emit(OpCodes.Call, baseConstructor);    // call base constructor

				ilGenerator.Emit(OpCodes.Ret);

				var type = typeBuilder.CreateType();
				_hubTypes.Add(hubName, type);
			}
		}
	}
}
