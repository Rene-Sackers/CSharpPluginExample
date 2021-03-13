using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using Newtonsoft.Json;

namespace CSharpPluginExample.Host.Helpers
{
	/// <summary>
	/// From: https://github.com/dotnet/runtime/issues/13283#issuecomment-686400972
	/// </summary>
	public static class NewtonsoftIsolationHelper
	{
		private static readonly string[] RequiredFrameworkAssemblyNames = {
			"netstandard",
			"System.ComponentModel.TypeConverter"
		};

		private static readonly Lazy<IEnumerable<string>> RequireFrameworkAssemblyLocations = new(() =>
		{
			// force newtonsoft to load in the default assembly load context, so that it in turn loads the required assemblies
			// there aren't the right hooks into the default assembly load context to get them on natural resolution
			JsonConvert.SerializeObject(new { });
			return AssemblyLoadContext.Default.Assemblies
				.Where(a => RequiredFrameworkAssemblyNames.Any(n => string.Equals(a.GetName().Name, n, StringComparison.OrdinalIgnoreCase)))
				.Select(o => o.Location);

		});

		public static void LoadRequiredFrameworkAssemblies(this AssemblyLoadContext loadContext)
		{
			foreach (var a in RequireFrameworkAssemblyLocations.Value)
			{
				loadContext.LoadFromAssemblyPath(a);
			}
		}
	}
}
