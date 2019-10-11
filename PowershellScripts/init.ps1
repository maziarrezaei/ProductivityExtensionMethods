using namespace System.IO
param($installPath, $toolsPath, $package, $project)

function ParseT4Content([String] $content)
{
    $regEx = [regex] '(?inm-s)//ToolsVersion:(?<Version>(?:\d+\.){2}\d+(-[\w|\d]+\.(\d+\.)*\d+)?)\s*$';

    $match = $regEx.Match($content);

    if (-not $match.Success)
    {
        return $null;
    }

    return @{
        Version       = $match.Groups['Version'].Value
        ConfigSection = $content.Substring(0, $match.Index)
        CodeSection   = $content.Substring($match.Index)
    };
}

function ApplyOldConfigToNew([String] $oldConfig, [String] $newConfig)
{
    # Reads the config values from the old config, and applies them to the new config
    # Returns the new updated config created.

    $variableRegex = [regex] '(?<=\s(?<Name>[a-zA-Z0-9_]+)\s*=\s*)(?<Value>true|false)\s*(?=;)';

    $oldConfigValues = $variableRegex.Matches($oldConfig) | % { [PSCustomObject]@{ 
                                                                    Name  = $_.Groups['Name'].Value;
                                                                    Value = $_.Groups['Value'].Value
                                                                }
    };

    return $variableRegex.Replace($newConfig,
        {
            $newConfigMatch = $args[0];

            $oldConfig = $oldConfigValues | where { $_.Name -eq $newConfigMatch.Groups['Name'].Value } | select -First 1;

            if ($oldConfig)
            {
                return $oldConfig.Value;
            }
            else
            {
                #do not replace,
                return $newConfigMatch.Groups['Value'].Value;
            }
        });
}

function EnsureFileAddedToProject([String] $filePath, $project)
{
    $projItem = $project.ProjectItems | where { $_.Properties | where { $_.Value -eq $filePath } };

    if (-not $projItem)
    {
        $projItem = $project.ProjectItems.AddFromFile($filePath);
    }


    $customToolsProp = $projItem.Properties | where { $_.Name -eq "CustomTool" } | select -First 1

    if($customToolsProp.Value -ne 'TextTemplatingFileGenerator')
    {
        $customToolsProp.Value='TextTemplatingFileGenerator'
    }
}

if(-not $project)
{
	# we are in an update scenario. For some stupid reason, nuget in Visual Studio calls init.ps1 of the old version when it is removing it to install the updated version.
	# it passes empty project, to make sure you have no clue what project is being updated either, so it's essentially useless. if not returning, 
	# an exception in old init.ps1 will result in not running the init.ps1 of the new package, which makes me wonder. 
	
	Write-Host 'Exiting old version of the init.ps1'

	exit 0;
}

$packageT4Path = "$toolsPath\..\assets\ProductivityExtension.methods.tt";

$t4Path = [Path]::Combine([Path]::GetDirectoryName($project.Properties.Item('FullPath').Value), "ProductivityExtensions.tt");

if (Test-Path $t4Path -PathType Leaf)
{
    #read the file, and only overwrite the code section
    $currentT4File = ParseT4Content (Get-Content $t4Path -Raw)

	if(-not $currentT4File)
	{
		throw "Cannot read the version information in T4 file in project. Make sure the file is not tampered with in wrong ways."
	}

    $packageT4File = ParseT4Content (Get-Content $packageT4Path -Raw);

	if(-not $packageT4File)
	{
		throw "Cannot read the version information from the package. This should be a bug in package."
	}
    
    if ($currentT4File.Version -ne $packageT4File.Version)
    {
	    Write-Host 'The current T4 file is from another version. Porting configuration.'

		$currentT4File.ConfigSection = ApplyOldConfigToNew -oldConfig $currentT4File.ConfigSection -newConfig $packageT4File.ConfigSection
    }

    Set-Content $t4Path -Value ($currentT4File.ConfigSection + $packageT4File.CodeSection)
}
else
{
    #copy the file to the project path. Include in project file if necessary.
    Copy-Item $packageT4Path $t4Path

    EnsureFileAddedToProject $t4Path $project
}