# ProductivityExtensionMethods
Extension methods done right! When installed, a [T4 Template](https://docs.microsoft.com/en-us/visualstudio/modeling/code-generation-and-t4-text-templates) file is added to the project that, based on the what you enable, generates useful extension methods for certain base class library types and common intefaces that greately increase productivity. Besides, it demonstrates an improved approach to distributing or re-using extension methods.

[**Visit FAQ page for more details.**](https://github.com/maziarrezaei/ProductivityExtensionMethods/wiki/FAQ)


## How to use
It is [available on nuget](https://www.nuget.org/packages/ProductivityExtensionMethods).

```
> Install-Package ProductivityExtensionMethods -IncludePrerelease
```

After referencing the package, a [T4 Template](https://docs.microsoft.com/en-us/visualstudio/modeling/code-generation-and-t4-text-templates) file is created in your project that will generate a C# .cs file. Modify top section of the file to enable certain category of extension methods. Nuget will take care of future updates.

## Important Notes
- If updating, make sure to open the ProductivityExtension.tt file and save to re-generate code. 
- The generated code works in C# projects only.
- The C# code contains C# 7.3 syntax in some categories. Make sure it is enabled in your project by going to Project Properties -> Build -> Advanced...
- The C# code contains *Nullable Reference Type* feature available in C# 8. It will only be generated if target framework is compatible with .Net Standard 2.1 and above.
- It is tested on latest versions of Visual Studio 2019/2017.

## Known Issues
- If the nuget package is removed for any reason then immediately added back, no .tt file will be added to the project. This is a bug in Visual Studio/Nuget. The initialization script for the nuget package is not run the second time until Visual Studio is restarted.
- If package is added to one project, then immediately to another, the second project doesn't get the .tt file. This is due to the same bug as above and VS should be restarted between adds. 







