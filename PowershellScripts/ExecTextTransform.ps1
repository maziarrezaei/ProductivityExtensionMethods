param([string] $T4Template= $(throw "The T4Template parameter is required."))

if(-not (Test-Path $T4Template -PathType Leaf))
{
	Write-Error $"File not found $T4Template"
	exit 1;
}

Write-Host 'Finding latest text transform utility...'

$commandFile= dir -Include TextTransform.exe -r -Path 'C:\Program Files (x86)\Microsoft Visual Studio' | Sort-Object -Property @{Expression={$_.VersionInfo.ProductVersion}; Descending= $true} | select -First 1

if(-not $commandFile)
{
	Write-Error $"Cannot find T4 transformation command."
	exit 2;
}
pushd .
cd (Split-Path $T4Template)
& $commandFile.FullName $T4Template
popd