
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$Project
)

$ErrorActionPreference = "Stop"

$repo = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$image = "pathofcultivation-dev:0.0.2"
$volume = "poc-nuget-packages"

$projectPath = (Resolve-Path -LiteralPath $Project).Path
$relativePath = [System.IO.Path]::GetRelativePath($repo, $projectPath)

if ($relativePath -eq ".." -or
    $relativePath.StartsWith("../") -or
    $relativePath.StartsWith("..\")) {
    throw "Project must be inside the repository."
}

$containerProject = $relativePath.Replace("\", "/")

Write-Host "Restoring: $containerProject" -ForegroundColor Cyan

docker run --rm `
    --network bridge `
    --mount "type=bind,source=$repo,target=/workspace" `
    --mount "type=volume,source=$volume,target=/home/developer/.nuget/packages" `
    --workdir /workspace `
    $image `
    dotnet restore $containerProject

if ($LASTEXITCODE -ne 0) {
    throw "NuGet restore failed with exit code $LASTEXITCODE"
}

Write-Host "NuGet restore completed." -ForegroundColor Green
