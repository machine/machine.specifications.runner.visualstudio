Param(
    [Parameter(Mandatory=$true)][string] $version,
    [Parameter(Mandatory=$true)][string] $manifestFile
)


if ((Test-Path $manifestFile) -ne $true) {
    throw "Unable to find vsix manifest file: ${manifestFile}"
}

Write-Host "Applying version ${version} to ${manifestFile}"

(Get-Content $manifestFile) | Foreach-Object {$_ -replace "<Version>.*?</Version>", "<Version>$version</Version>"} | Set-Content $manifestFile
