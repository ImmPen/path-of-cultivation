# Path of Cultivation — Codex Instructions

## Project
Path of Cultivation is a RimWorld 1.7 mod written in C# and XML.

- Target framework: .NET 10 (`net10.0`).
- Runtime: RimWorld 1.7 WaylithCLR.
- Build project: `Source/PathOfCultivation.csproj`.
- Compiled assemblies: `Assemblies/`.
- Game references: `References/` (local, not committed).
- Mod metadata: `About/About.xml`.

## Development Workflow
1. Inspect relevant files and requirements before proposing changes.
2. Explain the proposed solution and its scope.
3. Wait for explicit user approval before implementing.
4. Make small, focused changes.
5. Verify the changes and report results.
6. Do not proceed to the next stage without user confirmation.

## Scope
- Follow the existing project documentation.
- Keep MVP 0.1.0 separate from future features.
- Do not implement undocumented mechanics without approval.
- Prefer simple, maintainable solutions over premature abstractions.

## Code
- Use C# and XML appropriate for RimWorld 1.7.
- Do not assume RimWorld 1.7 uses the legacy Mono/.NET Framework runtime.
- Preserve existing namespaces and naming conventions.
- Do not modify unrelated files.
- Do not modify files under `References/`.

## Validation
- Run `dotnet build Source/PathOfCultivation.csproj --no-restore` after C# changes when the build environment permits.
- Report build failures and warnings.
- Do not claim runtime compatibility without an in-game test.
- Inspect `git diff` after implementation.

## Git
- Do not create commits, tags, branches, or push changes without explicit approval.
- Do not modify Git configuration.
- Never commit secrets, local game DLLs, or build artifacts.

## Communication
- Communicate with the user in Russian.
- Clearly distinguish confirmed facts, assumptions, and proposals.
- Ask before expanding the requested scope.
