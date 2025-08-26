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

## [1.0.4] - 2025-08-24
### Menu Wheel
- Added ability to position using `Pos` in `Space.Self`
- Properly documented `MenuWheel` and `MenuWheelButton`
- Added dedicated Builder for `MenuWheel`
- Fixed an issue with the `MenuWheel` not rendering correctly when interacting with popups
- Smaller adjustments

## [1.0.5] - 2025-08-26
### Hotbar
- Improved [Hotbar](Runtime/Components/Game/Hotbar.cs) scrolling behaviour to work on mouse scroll X and Y
- Added documentation to `Hotbar` and made it more generally accessible

## [1.0.5.1] - 2025-08-26
### Hotbar Events
- Added an UnityEvent which is fired every time the [Hotbar](Runtime/Components/Game/Hotbar.cs)'s selected slot is changed.
- Removed unnecessary files

