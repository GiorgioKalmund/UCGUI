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

## [1.0.6] - 2025-08-26
### Hotbar Events
- Added an UnityEvent which is fired every time the [Hotbar](Runtime/Components/Game/Hotbar.cs)'s selected slot is changed.
- Removed unnecessary files

## [1.0.7] - 2025-09-03
### Views and ScrollViews, Cleanup
- Added ScrollViews to UI builder to dramatically simplify usage
- Renamed `PopupComponent` to `ViewComponent` and added UI builder
- Improved `ViewComponent` functionalities
- Introduced `IEnabled`, which allows unified enabling / disabling
- Removed Game folder and consolidated it into `Demo`
- Removed Testing folder

## [1.0.8] - 2025-09-08
### Styles and Partials
- Added the capability to save preset styles using [AbstractStyle](Runtime/Components/Style/AbstractStyle.cs)
  - This currently includes [TextStyles](Runtime/Components/Style/TextStyle.cs) and [ButtonStyles](Runtime/Components/Style/ButtonStyle.cs).
  - [UCGUIStyles](Runtime/Components/Style/UCGUIStyles.cs) will offer some quick default styling options
  - Any component can be styled, but it is best paired with the [IStylable](Runtime/Components/Interface/IStylable.cs) interface for a uniform code style (*badum-ts*)
- Most classes are now labeled as `partial` allowing a simple and direct way to build on top of UCGUI without destroying the nice `UI.ComponentName(...)` syntax for example when using custom components


