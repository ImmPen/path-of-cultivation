# Path of Cultivation — Codex Instructions

## Project

Path of Cultivation is a RimWorld 1.7 mod written in C# and XML.

- Target framework: .NET 10 (`net10.0`).
- Runtime: RimWorld 1.7 WaylithCLR.
- Build project: `Source/PathOfCultivation.csproj`.
- Compiled assemblies: `Assemblies/`.
- Game references: `References/` (local, not committed).
- Mod metadata: `About/About.xml`.

## Sources of Truth

- Read the relevant project documentation before proposing or implementing changes.
- Use `docs/MVP_BACKLOG.md` to determine planned MVP work and dependencies.
- Treat the existing source code as evidence of what is actually implemented.
- Distinguish documented requirements from implemented and verified functionality.
- If documentation and implementation disagree, report the discrepancy instead of silently changing either.
- Do not mark a task complete solely because its documentation exists.

## Execution Modes

### Analyze Mode

Analyze mode is used to investigate requirements, evaluate solutions, and prepare implementation plans.

- Inspect relevant documentation and source files.
- Do not create, modify, rename, or delete project files.
- Do not execute commands that modify the repository.
- Identify dependencies, risks, and missing information.
- Recommend one small, independently verifiable implementation task when asked to select the next task.
- Provide a concrete plan for user approval.
- Do not begin implementation.

### Implement Mode

Implement mode is used to execute a task already approved by the user.

- Follow the provided task specification.
- Treat the task specification as the authorized scope, not as permission for unrelated improvements.
- Inspect relevant files before editing.
- Implement the approved changes without requesting redundant approval.
- Stop and report if a material ambiguity or scope conflict prevents safe implementation.
- Make small, focused changes.
- Validate the implementation and report the results.
- Do not continue into another task.

## Development Priorities

- Follow the existing project roadmap and MVP backlog.
- Keep MVP `0.1.0` separate from post-MVP features.
- Respect task dependencies and the current verified implementation state.
- When asked for the next gameplay implementation task, prioritize actionable MVP gameplay work whose prerequisites are satisfied.
- Do not repeatedly recommend completed environment setup, build verification, or documentation-only tasks unless they are necessary blockers or explicitly requested.
- Do not treat an outdated checklist entry as definitive proof that previously verified work was not completed.
- Report discrepancies between the backlog and verified project state.
- Do not silently update documentation or task statuses without authorization.
- Do not implement undocumented mechanics without approval.
- Prefer simple, maintainable solutions over premature abstractions.

## Code

- Use C# and XML appropriate for RimWorld 1.7.
- Do not assume RimWorld 1.7 uses the legacy Mono/.NET Framework runtime.
- Preserve existing namespaces and naming conventions.
- Follow existing project patterns where appropriate.
- Do not modify unrelated files.
- Do not modify files under `References/`.
- Avoid unnecessary dependencies and abstractions.
- Do not introduce gameplay features outside the approved task.

## Validation

- Run `dotnet build Source/PathOfCultivation.csproj --no-restore` after C# changes when the build environment permits.
- After Core calculation rule changes, run inside the agent container: `dotnet test tests/PathOfCultivation.Core.Tests/PathOfCultivation.Core.Tests.csproj --no-restore --configuration Release`.
- `tools/test.ps1` is for external Windows execution through Docker; use direct `dotnet` in the agent container, without nested Docker.
- If `project.assets.json` or restored packages are missing, report the specific blocker; preparation uses the existing restore process. Do not enable network access or add dependencies automatically.
- Report build failures and warnings.
- Inspect `git diff` after implementation.
- Check for unintended changes.
- Distinguish successful compilation from verified in-game behavior.
- Do not claim runtime compatibility without an in-game test.
- If validation cannot be performed, explain why and provide a manual verification procedure.
- Never report a test as passed unless it was actually executed.

## Git

- Do not create commits, tags, branches, or push changes without explicit approval.
- Do not modify Git configuration.
- Never commit secrets, local game DLLs, or build artifacts.
- Do not revert or overwrite unrelated user changes.
- Do not use destructive Git commands without explicit approval.

## Communication

- Communicate with the user in Russian.
- Clearly distinguish confirmed facts, assumptions, and proposals.
- Keep explanations concise and technically specific.
- Ask before expanding the requested scope.
- Do not proceed to the next development stage without user confirmation.

### Analyze Report

Include:

1. Current implementation state relevant to the task.
2. Recommended task and why it is next.
3. Dependencies and potential blockers.
4. Proposed files and changes.
5. Verification plan.
6. Open questions requiring user approval.

### Implement Report

Include:

1. Implemented changes.
2. Files modified.
3. Build and validation results.
4. Known limitations or unresolved issues.
5. Recommended next step, without implementing it.