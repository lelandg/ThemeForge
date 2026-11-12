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

- CI runs `gitleaks`, `osv-scanner` and `zizmor` on every PR, every push to `main`, and
  weekly (`.github/workflows/security.yml`). osv-scanner reads
  `ThemeForge.csproj` directly; a committed `packages.lock.json` is NOT required
  for NuGet (verified locally with osv-scanner 2.7.0: 2 packages scanned,
  exit 0). No npm jobs: there is no `package-lock.json`.
- Dependabot: `nuget` plus `github-actions` (`.github/dependabot.yml`). Alerts
  are a separate repo setting and must be switched on for the config to matter;
  Check it by HTTP STATUS, not by exit code. `gh api` exits 0 on success and 1
  on any failure; it never exits 404 or 403. `<owner>/<repo>` is also shell
  redirection, so the placeholder form cannot be pasted at all -- substitute
  the real slug and read the status line:
  `gh api -i -X GET repos/OWNER/REPO/vulnerability-alerts | head -1`
  204 = on, 404 = off, 403 = the token has no admin here (which is not "off").
- SHA-pinned actions get **no Dependabot alerts**: GitHub generates them only
  for actions on semantic versions (docs: "About Dependabot alerts"). So no
  security-update PR can ever fire for a pinned action, an
  `applies-to: security-updates` group over github-actions is dead config, and
  the weekly version update is their only automatic remediation -- which is why
  the github-actions block carries no semver-major ignore. DETECTION comes
  from the `zizmor` job in `security.yml`: it maps each SHA pin to its
  release tag and fails on a known-vulnerable, impostor or unpinned action.
  osv-scanner's `github/actions` plugin only lists the pinned actions: with
  osv-scanner 2.7.0 it did not flag `tj-actions/changed-files`
  (GHSA-mrrh-fwg8-r2c3) at a vulnerable SHA or tag. zizmor looks up a SHA
  pin by the longest tag on that commit, so the job also fails unless that
  tag is a plain release version (v1, v1.2.3): an untagged commit, a
  non-version tag or a path-prefixed tag would match no advisory. Pin
  released versions.
- Alerts and automatic security **updates** are two switches, not one. Alerts
  only report; the half that opens PRs is `automated-security-fixes`. It is
  deliberately OFF: this repo took its first scan with a pre-existing advisory
  backlog, and enabling it would open that backlog as PRs at once.
- PR reviewers configured for this repo: **Codex** (run locally against the
  committed branch before opening the PR) and **CodeRabbit**
  (`coderabbitai[bot]`). How to read their checks -- which one re-reviews a
  push, why a green check does not mean "no findings", and which label skips
  which bot -- is in the global policy cited above and is deliberately not
  repeated here.
## Pointers

- `README.md` — feature overview and theme model.
