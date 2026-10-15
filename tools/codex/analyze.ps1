
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$Task
)

$ErrorActionPreference = "Stop"

$repo = (Resolve-Path (Join-Path $PSScriptRoot "../..")).Path
$image = "pathofcultivation-dev:0.0.2"
$volume = "pathofcultivation-codex-home"

docker run --rm `
    --read-only `
    --cap-drop ALL `
    --security-opt no-new-privileges `
    --tmpfs /tmp:rw,nosuid,nodev,size=64m `
    --env GIT_CONFIG_COUNT=1 `
    --env GIT_CONFIG_KEY_0=safe.directory `
    --env GIT_CONFIG_VALUE_0=/workspace `
    --mount "type=bind,source=$repo,target=/workspace,readonly" `
    --mount "type=volume,source=$volume,target=/home/developer/.codex" `
    --workdir /workspace `
    $image `
    codex exec `
	    --model gpt-6-luna `
        --sandbox danger-full-access `
        $Task

if ($LASTEXITCODE -ne 0) {
    throw "Codex exited with code $LASTEXITCODE"
}
