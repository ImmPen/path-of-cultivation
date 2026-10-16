
param(
	[ValidateSet("Debug", "Release")]
	[string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path

docker info --format '{{.ServerVersion}}' *> $null

if ($LASTEXITCODE -ne 0) {
	throw "Docker Engine недоступен. Запусти Docker Desktop."
}

docker run --rm `
	--network none `
	--mount "type=bind,source=$repoRoot,target=/workspace" `
	--workdir /workspace `
	pathofcultivation-dev:0.0.2 `
	dotnet build Source/PathOfCultivation.csproj `
	--no-restore `
	--configuration $Configuration

if ($LASTEXITCODE -ne 0) {
	throw "Сборка Path of Cultivation завершилась с ошибкой."
}

Write-Host "Build succeeded ($Configuration)." -ForegroundColor Green
