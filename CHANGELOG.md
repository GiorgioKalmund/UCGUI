## [1.0.0] - 2025-08-23
### First Release
- Adds an codable interface for Unity's UGUI
- Created essential codable components for UI use
- Added a variety of services, allowing for easier use and access of certain UI aspects (`UCGUI.Services`).
- Added Demo material (`UCGUI.Demo`).
- Added Testing material (`UCGUI.Testing`).

## [1.0.1] - 2025-08-23
### Renamed Shortcuts
- Renamed the shortcut builder to 'UI' to remove duplicate referencing when using it.

## [1.0.2] - 2025-08-23
### Public Extensions
- Fixed the ComponentExtensions class not being public, thus scripts not being able to access it.

## [1.0.3] - 2025-08-23
### Own Extensions
- Made `ComponentExtension` its own class, to allow IDEs to detect it properly 
