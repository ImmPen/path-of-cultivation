
param(
    [Parameter(Mandatory = $true, Position = 0, ParameterSetName = "Text")]
    [ValidateNotNullOrEmpty()]
    [string]$Task,

    [Parameter(Mandatory = $true, ParameterSetName = "File")]
    [ValidateNotNullOrEmpty()]
    [string]$TaskFile
)

$ErrorActionPreference = "Stop"

# UTF-8 for communication with Docker and Codex
[Console]::InputEncoding = New-Object System.Text.UTF8Encoding($false)
[Console]::OutputEncoding = New-Object System.Text.UTF8Encoding($false)
$OutputEncoding = New-Object System.Text.UTF8Encoding($false)

$repo = (Resolve-Path (Join-Path $PSScriptRoot "../..")).Path

if ($PSCmdlet.ParameterSetName -eq "File") {
    $resolvedTaskFile = Resolve-Path -LiteralPath $TaskFile -ErrorAction Stop

    if ($resolvedTaskFile.ProviderPath -notlike "*.md") {
        throw "TaskFile must be a Markdown (.md) file."
    }

    $Task = [System.IO.File]::ReadAllText(
        $resolvedTaskFile.ProviderPath,
        [System.Text.Encoding]::UTF8
    )
}

if ([string]::IsNullOrWhiteSpace($Task)) {
    throw "Analyze task is empty."
}

$image = "pathofcultivation-dev:0.0.2"
$volume = "pathofcultivation-codex-home"

$logsDir = Join-Path $repo ".codex-tasks\logs"
New-Item -ItemType Directory -Path $logsDir -Force | Out-Null

$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$sessionLog = Join-Path $logsDir "$timestamp-analyze-session.log"
$resultFile = Join-Path $logsDir "$timestamp-analyze-result.md"

Write-Host "Session log: $sessionLog" -ForegroundColor Cyan
Write-Host "Result: $resultFile" -ForegroundColor Cyan

$ErrorActionPreference = "Continue"

docker run --rm `
    --interactive=false `
    --read-only `
    --cap-drop ALL `
    --security-opt no-new-privileges `
    --tmpfs "/tmp:rw,nosuid,nodev,size=64m" `
    --env GIT_CONFIG_COUNT=1 `
    --env GIT_CONFIG_KEY_0=safe.directory `
    --env GIT_CONFIG_VALUE_0=/workspace `
    --mount "type=bind,source=$repo,target=/workspace,readonly" `
    --mount "type=bind,source=$logsDir,target=/workspace/.codex-tasks/logs" `
    --mount "type=volume,source=$volume,target=/home/developer/.codex" `
    --workdir /workspace `
    $image `
    codex exec `
    --model gpt-6-luna `
    --sandbox danger-full-access `
    --output-last-message "/workspace/.codex-tasks/logs/$timestamp-analyze-result.md" `
    -- $Task 2>&1 |
ForEach-Object {
				if ($_ -is [System.Management.Automation.ErrorRecord]) {
        $_.ToString()
				}
    else {
        $_
				}
} |
Tee-Object -FilePath $sessionLog
		
$dockerExitCode = $LASTEXITCODE

$ErrorActionPreference = "Stop"

if ($dockerExitCode -ne 0) {
    throw "Codex Analyze failed with exit code $dockerExitCode. See $sessionLog"
}

Write-Host "Analysis completed." -ForegroundColor Green
Write-Host "Session: $sessionLog"
Write-Host "Result: $resultFile"
