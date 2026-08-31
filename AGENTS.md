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

## Pointers

- `README.md` — feature overview and theme model.
