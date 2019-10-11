# ProductivityExtensionMethods
Extension methods done right! A series of useful extension methods based on many BCL types and you can opt-in to enable in your solution. No more messy big junk classes!

**Currently only for C# language.**
## How to install
It is [available on nuget](https://www.nuget.org/packages/ProductivityExtensionMethods).

After Referencing the package, it should create a [T4 Template](https://docs.microsoft.com/en-us/visualstudio/modeling/code-generation-and-t4-text-templates) file in your project that will generate a C# .cs file. Modify top section of the file to enable certain category of extension methods.

## How is it different from other Extension Method DLLs?
This is NOT a DLL assembly! 

DLLs are not good, because:
- A DLL that only contains extension methods, in essence a series of so-called "utility" or "helper" functions, is not a good design approach. It's just ugly, and I'm sure I don't need to explain more.
- It is an extra totally unnecessary addition to the deployment, especially if you need to deal with strong names and/or signing the assembly. This is especially not justified when you did not use the extension methods extensively.
- Referencing such DLL adds in hundreds of extension methods that pollute your IntelliSense. No way to choose only what you need on a project-level basis. 
- You need to add a reference in all your projects in the solution to have it available everywhere. Bad practice, on so many levels.
- You may need to add an extra using statement in ALL your code files to make them available everywhere.

## How ProductivityExtensionMethods tackles the problem?
- Referencing nuget package only adds one [T4 Template](https://docs.microsoft.com/en-us/visualstudio/modeling/code-generation-and-t4-text-templates) file to your project.
- The T4 template generates a C# code file (.cs) in that project. It is a single partial static class, under the default namespace of the assembly.
- You add the nuget package to the project that is referenced by all your other projects. The methods are now available everywhere!
- The top section of the T4 template file has a *configuration* that you can use to switch on only the set of the methods that you need.
- When new version of this package arrives, you just update the package. It automatically takes care of porting your configuration to the new version.
- It automatically picks up whether or not [nullable reference types](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/nullable-reference-types) are enabled in your project and uses that feature as necessary.


## Important Notes
- The generated class is in C#, so it only works in C# projects.
- The C# code contains C#8 syntax for some categories.
- This package is only tested for .net standard project in Visual Studio 16.3 though the implementation should work on other project types as well.

## Known Issues
- If you reference the package but don't see any items added to your project, make sure the project was saved in the first place. This is a bug that will be fixed in a future versions.
- If you removed the package for any reason, and immediately added the package again, no file will be added to the project. This is a bug in Visual Studio/Nuget that when you remove and immediately add again, the initialization script for the nuget package is not called. (Observed in Visual Studio 2019 16.3)