
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$Task
)

$ErrorActionPreference = "Stop"

# UTF-8 for communication with Docker and Codex
[Console]::InputEncoding = New-Object System.Text.UTF8Encoding($false)
[Console]::OutputEncoding = New-Object System.Text.UTF8Encoding($false)
$OutputEncoding = New-Object System.Text.UTF8Encoding($false)

$repo = (Resolve-Path (Join-Path $PSScriptRoot "../..")).Path
$image = "pathofcultivation-dev:0.0.2"
$volume = "pathofcultivation-codex-home"

$logsDir = Join-Path $repo ".codex-tasks\logs"
New-Item -ItemType Directory -Path $logsDir -Force | Out-Null

$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$sessionLog = Join-Path $logsDir "$timestamp-implement-session.log"

# Resolve task file relative to repository root
$taskPath = Join-Path $repo $Task

if (Test-Path -LiteralPath $taskPath -PathType Leaf) {
    $relativeTask = [System.IO.Path]::GetRelativePath($repo, (Resolve-Path $taskPath).Path)
    $relativeTask = $relativeTask.Replace('\', '/')

    Write-Host "Task file: $relativeTask" -ForegroundColor Cyan

    $Task = @"
Прочитай файл /workspace/$relativeTask.
Это согласованное техническое задание на реализацию.

Изучи связанные требования в docs/ и существующий код.
Выполни только описанные в задании изменения.
Не расширяй область работ.
После реализации проверь изменения и сообщи результат.
Не создавай Git-коммиты.
"@
}

Write-Host "Codex implementation mode" -ForegroundColor Yellow
Write-Host "Repository: $repo"
Write-Host "Model: gpt-6.1-sol"
Write-Host "WARNING: Codex will have write access to the repository." -ForegroundColor Yellow

$confirmation = Read-Host "Continue? (yes/no)"

if ($confirmation -ne "yes") {
    Write-Host "Cancelled."
    exit 0
}

$ErrorActionPreference = "Continue"

docker run --rm `
    --interactive=false `
    --read-only `
    --cap-drop ALL `
    --security-opt no-new-privileges `
    --tmpfs /tmp:rw,nosuid,nodev,size=64m `
    --env GIT_CONFIG_COUNT=1 `
    --env GIT_CONFIG_KEY_0=safe.directory `
    --env GIT_CONFIG_VALUE_0=/workspace `
    --mount "type=bind,source=$repo,target=/workspace" `
    --mount "type=volume,source=$volume,target=/home/developer/.codex" `
    --mount "type=volume,source=poc-nuget-packages,target=/home/developer/.nuget/packages" `
    --env "NUGET_PACKAGES=/home/developer/.nuget/packages" `
    --workdir /workspace `
    $image `
    codex exec `
        --model gpt-6.1-sol `
        --sandbox danger-full-access `
        -- $Task 2>&1 |
    ForEach-Object {
        if ($_ -is [System.Management.Automation.ErrorRecord]) {
            $_.ToString()
        } else {
            $_
        }
    } |
    Tee-Object -FilePath $sessionLog

$dockerExitCode = $LASTEXITCODE
$ErrorActionPreference = "Stop"

if ($dockerExitCode -ne 0) {
    throw "Codex Implement failed with exit code $dockerExitCode. See $sessionLog"
}

Write-Host "Implementation completed." -ForegroundColor Green
Write-Host "Session: $sessionLog"