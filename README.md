# ProductivityExtensionMethods
Extension methods done right!  A select set of extension methods on some core .net class library types with the mindset to keep it minimal to be able to standardize in any team. Enabled by the different approach to distribution, you will be able to turn off the subset of methods that you are not likely to use in your project or standardize in your team. No more big messy extension classes with hundreds of extension methods that you will never use!

**Currently only for C# language.**
## How to use
It is [available on nuget](https://www.nuget.org/packages/ProductivityExtensionMethods).

After referencing the package, a [T4 Template](https://docs.microsoft.com/en-us/visualstudio/modeling/code-generation-and-t4-text-templates) file is created in your project that will generate a C# .cs file. Modify top section of the file to enable certain category of extension methods. Nuget will take care of future updates.

## Important Notes
- The generated class is in C#, so it only works in C# projects.
- The C# code contains C#8 syntax for some categories.
- This package is only tested for .net standard project in Visual Studio 16.3 though the implementation should work on other project types as well.

## Known Issues
- If you reference the package but don't see any items added to your project, make sure the project was saved in the first place. This is a bug that will be fixed in a future versions.
- If you removed the package for any reason, and immediately added the package again, no file will be added to the project. This is a bug in Visual Studio/Nuget that when you remove and immediately add again, the initialization script for the nuget package is not called. (Observed in Visual Studio 2019 16.3)

## How is it different from other Extension Method DLLs?
This is NOT a DLL assembly! 

DLLs are not good, because:
- A DLL that only contains extension methods, in essence a big lump of so-called "utility" or "helper" functions, is not a good design approach. It's just ugly, and I'm sure I don't need to explain more.
- It is an extra totally unnecessary addition to the deployment, especially if you need to deal with strong names and/or signing the assembly. This is especially not justified when you did not use the extension methods extensively.
- Referencing such DLL adds in hundreds of extension methods that pollute your IntelliSense. No way to choose only what you need on a project-level basis, and no clear way to define team's coding standards around them.
- You need to add a reference in all your projects in the solution to have it available everywhere. Bad practice, on so many levels.
- You may need to add an extra using statement in ALL your code files to make them available everywhere.

## How the ProductivityExtensionMethods tackles the problem?
- Referencing nuget package only adds one [T4 Template](https://docs.microsoft.com/en-us/visualstudio/modeling/code-generation-and-t4-text-templates) file to your project.
- The T4 template generates a C# code file (.cs) in that project. It is a single partial static class, under the default namespace of the assembly.
- Add the nuget package to the assembly that is referenced by other assemblies. The methods are now available everywhere!
- The top section of the T4 template file has a *configuration* that you can use to switch on only the set of the methods that you need or plan to standardize in your team.
- When new version of this package arrives, update the package. It automatically takes care of porting your configuration to the new version.

## Why so few methods compared to other libraries?
The driving mindset in selection of the majority of the extension methods here, is the ability to introduce them as coding guidelines within the team, and possibly across the teams in bigger companies. Such extension methods not only aim to improve productivity of individual team members, but also improve the readability of the code. This cannot be achieved if only one or two developers who happen to come across the library, or are more patient sifting through the methods, use them and the rest of the team completely ignore. By keeping the number of the extension methods at bare minimum, and giving the ability to even narrow them down to smaller subsets, the team can more easily standardize and use them. It will become a habits for more developers. This will eventually result in a better maintainable code. Some examples of such methods are: string.IsBlank(), val.LimitH(), val.IsBetween(),  and IEnumerable<IGrouping<,>>.ToDictionary().

With this philosophy, the possibility will be reserved to deprecate or even drop some methods between the major releases. This is the deeper philosophy behind why the library is distributed as source code. An upgrade won't break any binary and the developers can take the deleted method, in case they used it, and put it in their own partial class and keep it if they wish so. This way, the library becomes leaner over time, and can provide improved best practices as it evolves. 

The second factor in picking/developing the methods is simply sharing some complex code that is likely to be used, but are not available anywhere else on the Internet. One example is the ToHierarchy() method for converting a flat list into a tree hierarchy which does it in a very unique way. However, such methods are the strangest to the core philosophy and you should not expect to see many of them in future releases. 

## Ho can I contribute? I don't see the method that I like!
Contributions are very welcome through either suggestions, sending code or even better pull requests. 

Please report the bugs on github! The project is now actively in development and is in desperate need of testing on multiple environments. If you see it is not working, please open an issue on Github.

The project is in need of XML documentation and automated testing. These are the next targets for development, but contributions of any kind will be welcomed!

If you are recommending extension methods, please keep the philosophy mentioned above in mind. 

For the pull request to be accepted:
- The comments on the commits must be in English.
- There should not be any Merge commits. Please squash such commits or use rebase to get the latest. Your changes will most likely be rebased into upstream as well.
- Please keep your commits clean. If commits are fixing one another, they should be squashed.






