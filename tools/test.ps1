param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path

docker run --rm `
    --network none `
    --mount "type=bind,source=$repoRoot,target=/workspace" `
    --mount "type=volume,source=poc-nuget-packages,target=/home/developer/.nuget/packages,readonly" `
    --env "NUGET_PACKAGES=/home/developer/.nuget/packages" `
    --workdir /workspace `
    pathofcultivation-dev:0.0.2 `
    dotnet test tests/PathOfCultivation.Core.Tests/PathOfCultivation.Core.Tests.csproj `
    --configuration $Configuration `
    --no-restore

if ($LASTEXITCODE -ne 0) {
    throw "Offline tests failed with exit code $LASTEXITCODE"
}
