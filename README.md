# ProductivityExtensionMethods
Extension methods done right! When installed, a [T4 Template](https://docs.microsoft.com/en-us/visualstudio/modeling/code-generation-and-t4-text-templates) file is added to the project that, based on the what you enable, generates useful extension methods for certain base class library types and common intefaces that greately increase productivity. Besides that, it demonstrates an improved method to distribute extension methods.

[**Visit FAQ page for more details.**](https://github.com/maziarrezaei/ProductivityExtensionMethods/wiki/FAQ)


## How to use
It is [available on nuget](https://www.nuget.org/packages/ProductivityExtensionMethods).

```
> Install-Package ProductivityExtensionMethods -Version 1.0.0-beta.5
```

After referencing the package, a [T4 Template](https://docs.microsoft.com/en-us/visualstudio/modeling/code-generation-and-t4-text-templates) file is created in your project that will generate a C# .cs file. Modify top section of the file to enable certain category of extension methods. Nuget will take care of future updates. Ignore the generated file in your source control.

## Important Notes
- The generated class is in C#, so it only works in C# projects.
- The C# code contains C# 7.3 syntax in some categories. Make sure it is enabled in your project by going to Project Properties -> Build -> Advanced...
- The C# code contains *Nullable Reference Type* feature available in C# 8. It will only be generated if target framework is .Net Core 3.0 or .Net Standard 2.1.
- This package is tested on latest versions of Visual Studio 2019/2017.

## Known Issues
- If the nuget package is removed for any reason then immediately added back, no .tt file will be added to the project. This is a bug in Visual Studio/Nuget. The initialization script for the nuget package is not run the second time until Visual Studio is restarted.
- If package is added to one project, then immediately to another, the second project doesn't get the .tt file. This is due to the same bug as above and VS should be restarted between adds. 







