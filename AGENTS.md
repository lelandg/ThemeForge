# ThemeForge — agent guide

ThemeForge is a .NET 8 WPF app (`net8.0-windows`) that lets users create, edit, and apply themes with live preview. The `Themes/` folder is designed as a drop-in theming system for other WPF projects.

## Hard rules

None beyond the global house rules.

## Gotchas

- The target is `net8.0-windows` with `UseWindowsForms` on (.NET 8 is past end of support as of 2026-11-10; the retarget is pending and is a change to `ThemeForge.csproj`, not to this file). Build and run from native Windows (`dotnet build`, `dotnet run`); WSL cannot build WPF.
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

Policy lives in `~/.config/agents/AGENTS.md` -> "Dependencies, advisories, and
scanning" (full text: `~/.claude/instructions/security-scanning.md`) and "Model
delegation & cross-provider review". It is not restated here; read it there.
What is specific to this repo:

- CI runs `gitleaks`, `osv-scanner` and `zizmor` on every PR, every push to `main`, and
  weekly (`.github/workflows/security.yml`). osv-scanner reads
  `ThemeForge.csproj` directly; a committed `packages.lock.json` is NOT required
  for NuGet (verified locally with osv-scanner 2.7.0: 2 packages scanned,
  exit 0). No npm jobs: there is no `package-lock.json`.
- Dependabot: `nuget` plus `github-actions` (`.github/dependabot.yml`). Version
  updates run from the config alone. Security updates also need Dependabot
  alerts, a separate repo setting. Check it by HTTP STATUS, not by exit code. `gh api` exits 0 on success and 1
  on any failure; it never exits 404 or 403. `<owner>/<repo>` is also shell
  redirection, so the placeholder form cannot be pasted at all -- substitute
  the real slug and read the status line:
  `gh api -i -X GET repos/OWNER/REPO/vulnerability-alerts | head -1`
  204 = on, 403 = the token has no admin here (which is not "off"), 404 = off
  only when an admin token gets it: GitHub also answers 404 to a token without
  the permission, so confirm with an admin token first.
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
  pin by one tag: for a root action (owner/repo@sha) the longest tag
  without a `/`, for a sub-action the longest tag under its path or its
  last directory (else the longest of all). The job fails unless that tag, and any tag tied
  with it, is a plain release version (v1, v1.2.3): an untagged commit,
  a non-version tag or a path-prefixed tag matches no advisory. Pin
  released versions.
- Alerts and automatic security **updates** are two switches, not one. Alerts
  only report; the half that opens PRs is `automated-security-fixes`. It is
  deliberately OFF: this repo took its first scan with a pre-existing advisory
  backlog, and enabling it would open that backlog as PRs at once.
- **PR reviewers.** The review council in the global rules applies here; its
  members are whatever `~/.config/agents/council.toml` seats, and the runner
  and commands are in `~/.claude/instructions/model-delegation.md`. This repo
  adds **CodeRabbit** (`coderabbitai[bot]`, config `.coderabbit.yaml`): it
  reviews automatically and re-reviews every push, skips drafts and Dependabot
  or Renovate PRs, and is skipped by the `no-ai-review` label, which must
  exist in the repo and be on the PR when it opens. A green `CodeRabbit` check
  does not mean a review ran or that it found nothing: read the check run's
  output title (`Finished` = reviewed) on the PR's head and the inline
  comments. The commands for that, and for recovering an unreviewed head, are
  in the comments of `.coderabbit.yaml` and in
  `~/.claude/instructions/pr-review-automation.md`. There is no GrepTile
  config here. The list is additive, not closed: nothing named here replaces
  a council member.

## Pointers

- `README.md` — feature overview and theme model.
