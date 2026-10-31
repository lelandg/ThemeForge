# ThemeForge — agent guide

ThemeForge is a .NET 8 WPF app (`net8.0-windows`) that lets users create, edit, and apply themes with live preview. The `Themes/` folder is designed as a drop-in theming system for other WPF projects.

## Hard rules

None beyond the global house rules.

## Gotchas

- The target is `net8.0-windows` with `UseWindowsForms` on. Build and run from native Windows (`dotnet build`, `dotnet run`); WSL cannot build WPF.
- WPF and Windows Forms share control names (`Button`, `MessageBox`, `Application`). Qualify the namespace or you get ambiguity errors and warnings.
- Themed controls bind with `DynamicResource`. A `StaticResource` does not update on theme switch.
- User settings and saved themes live in `%APPDATA%\ThemeForge\` (`Themes\` subfolder for theme files). Delete that folder to reset state.

## Conventions

- `ThemeManager.Current` (`Themes/ThemeManager.cs`) is the single entry point for all theme operations.
- Keep `Themes/` self-contained. Other projects integrate by copying that folder, so do not add dependencies from `Themes/` to files outside it.
- Built-in theme dictionaries: `Themes/Dark.xaml`, `Themes/Light.xaml`, `Themes/SharedResources.xaml`.
- Theme serialization uses Newtonsoft.Json (`Themes/ThemeData.cs`).

## Build / test

- No test project exists. Add one with xUnit or NUnit if you need tests.

## Scanning & review

Policy lives in `~/.config/agents/AGENTS.md` -> "Dependency, code, and secret
scanning" and "Model delegation & cross-provider review". It is not restated
here; read it there. What is specific to this repo:

- CI runs `gitleaks` and `osv-scanner` on every PR, every push to `main`, and
  weekly (`.github/workflows/security.yml`). osv-scanner reads
  `ThemeForge.csproj` directly; a committed `packages.lock.json` is NOT required
  for NuGet (verified locally with osv-scanner 2.7.0: 2 packages scanned,
  exit 0). No npm jobs: there is no `package-lock.json`.
- Dependabot: `nuget` plus `github-actions` (`.github/dependabot.yml`). Alerts
  are a separate repo setting and must be switched on for the config to matter.
- PR reviewers configured for this repo: **Codex** (run locally against the
  committed branch before opening the PR) and **CodeRabbit**
  (`coderabbitai[bot]`). How to read their checks -- which one re-reviews a
  push, why a green check does not mean "no findings", and which label skips
  which bot -- is in the global policy cited above and is deliberately not
  repeated here.
## Pointers

- `README.md` — feature overview and theme model.
