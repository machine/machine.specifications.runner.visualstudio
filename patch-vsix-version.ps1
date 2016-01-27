Param(
    [Parameter(Mandatory=$true)][string] $version,
    [Parameter(Mandatory=$true)][string] $manifestFile
)


if ((Test-Path $manifestFile) -ne $true) {
    throw "Unable to find vsix manifest file: ${manifestFile}"
}

Write-Host "Applying version ${version} to ${manifestFile}"

 -replace '<Identity (.*?) Version=".*?"','<Identity $1 Version="$version"'

(Get-Content $manifestFile) | Foreach-Object {$_ -replace '<Identity (.*?) Version=".*?"',"<Identity `$1 Version=""$version"""} | Set-Content $manifestFile
