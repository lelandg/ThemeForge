# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ThemeForge is a .NET 8.0 WPF application that provides a comprehensive theming system for Windows desktop applications. It allows users to create, customize, and apply themes with live preview capabilities.

## Build Commands

```bash
# Build the project
dotnet build

# Run the application
dotnet run

# Restore packages
dotnet restore

# Clean build artifacts
dotnet clean

# Publish for release
dotnet publish -c Release
```

## Architecture

### Core Components

- **ThemeManager** (`Themes/ThemeManager.cs`): Singleton pattern managing all theme operations. Access via `ThemeManager.Current`
- **Theme System** (`Themes/Theme.cs`, `Themes/ThemeData.cs`): Composite pattern with WindowTheme and MessageBoxTheme components
- **Dialog System** (`Themes/Dialogs/`): Custom themed dialogs including file dialogs, color picker, and message boxes
- **Settings** (`Utilities/SettingsManager.cs`): JSON-based configuration persistence using Newtonsoft.Json

### Key Directories

- `Themes/` - Core theming system with theme management, custom dialogs, and style definitions
- `Themes/Dialogs/` - Custom dialog implementations (file dialogs, color picker, breadcrumb navigation)
- `Themes/Styles/` - WPF style definitions for common controls
- `Utilities/` - Settings management and file system integration
- `Resources/` - Message box icons (Error.png, Info.png, Question.png, Warning.png)

### Entry Points

- **Application**: `App.xaml` and `App.xaml.cs` - Application startup with global resource dictionaries
- **Main Window**: `MainWindow.xaml` - Primary UI demonstrating theme capabilities
- **Theme Manager UI**: `Themes/ThemeManagerWindow.xaml` - Theme configuration interface

## Development Patterns

- **Data Binding**: Extensive use of WPF `DynamicResource` for theme switching
- **Settings Storage**: `%APPDATA%/ThemeForge/` directory for user settings
- **Resource Dictionaries**: Theme definitions in `Dark.xaml`, `Light.xaml`, and `SharedResources.xaml`
- **Integration**: Copy `Themes/` folder to integrate theming system into other WPF projects

## Technical Notes

- **Framework**: .NET 8.0 targeting `net8.0-windows`
- **UI Technologies**: WPF with Windows Forms integration enabled
- **Dependencies**: Newtonsoft.Json for theme serialization
- **Known Issues**: Namespace conflicts between WPF and Windows Forms controls may cause build warnings

## Testing

No dedicated test project exists. Tests should be added manually using xUnit or NUnit framework.