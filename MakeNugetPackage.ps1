cd $PSScriptRoot

$t4FilePath = '.\ProductivityExtensionMethods\ProductivityExtension.methods.tt';

if (-not (Test-Path $t4FilePath -PathType Leaf))
{
    Write-Error 'Cannot find ProductivityExtension.methods.tt. Open the solution in visual studio and save the t4 template file to generate this file.'
    exit 1;
}

try
{
    $node = Select-Xml -Path .\ProductivityExtensionMethods.nuspec -XPath '/package/metadata/version';
    $version = $node.Node.InnerXML;

    if ([String]::IsNullOrWhiteSpace($version))
    {
        throw "";
    }
}
catch
{
    Write-Error 'Unable to read the version from ProductivityExtensionMethods.nuspec. Ensure version is there!'
    exit 1;
}

$input = Read-Host "Create nuget package with version ($version)? (y/n)"

if ($input.ToLower() -ne 'y')
{
    exit 0;
}

$fileContent = Get-Content $t4FilePath -Raw

$fileContent = $fileContent -replace 'VersionPlaceholder{D8B1B561-500C-4086-91AA-0714457205DA}', $version

Set-Content $t4FilePath -Value $fileContent

if (Test-Path './nuget.exe')
{
    ./nuget pack ProductivityExtensionMethods.nuspec
}
elseif (Get-Command nuget -ErrorAction SilentlyContinue)
{
    nuget pack ProductivityExtensionMethods.nuspec
}
else
{
    Write-Error 'nuget.exe not found. Please either copy next to the script, or add to the Path environment variable.'
    exit 1;
}


