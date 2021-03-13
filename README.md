# .NET 5 plugin DLL load/unload example
This project demonstrates how to properly load and unload plugin assemblies in a .NET Core 3.1 or .NET 5.0+ project using `AssemblyLoadContext`.

I had major trouble trying to unload assemblies, so I created this test project to test some stuff out. It's *very* finicky.

Useful documentation:  
[Create a .NET Core application with plugins](https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support)  
[How to use and debug assembly unloadability in .NET Core](https://docs.microsoft.com/en-us/dotnet/standard/assembly/unloadability)

## Plugin project
In the plugin project's .csproj, you'll want to ensure a couple of things.

#1: `CopyLocalLockFileAssemblies` is set to `true`
```xml
<PropertyGroup>
	<TargetFramework>net5.0</TargetFramework>
	<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
</PropertyGroup>
```
This will cause all referenced packages/libraries to be output to the plugin's bin folder.

#2: Make sure the referenced host/plugin project is set to `Private: false` and `ExcludeAssets: runtime`
```xml
<ItemGroup>
	<ProjectReference Include="..\CSharpPluginExample.Host\CSharpPluginExample.Host.csproj">
		<Private>false</Private>
		<ExcludeAssets>runtime</ExcludeAssets>
	</ProjectReference>
</ItemGroup>
```
This will prevent it outputting the host assembly to the plgin assembly's bin folder.

## JSON.Net issues
As described in [issue #13283 on dotnet/runtime](https://github.com/dotnet/runtime/issues/13283), there's some issues when unloading a plugin that has used JSON.Net's serialize/deserialize methods.

This is resolved using code from [this comment](https://github.com/dotnet/runtime/issues/13283#issuecomment-686400972), see: [NewtonsoftIsolationHelper.cs](https://github.com/Rene-Sackers/CSharpPluginExample/blob/master/src/CSharpPluginExample/CSharpPluginExample.Host/Helpers/NewtonsoftIsolationHelper.cs), called on [L#44 in Program.cs](https://github.com/Rene-Sackers/CSharpPluginExample/blob/master/src/CSharpPluginExample/CSharpPluginExample.Host/Program.cs#L44)

Similar issues may exist with other libraries.

## Other issues
There's several other very specific issues that have been documented in the code. For example, I need to do a
```csharp
await Task.Delay(1);
```
after unloading the context.

Also make sure you unhook any events on unload.