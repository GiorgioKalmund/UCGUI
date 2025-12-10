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

## [1.0.9] - 2025-09-09
### Quality of Life
- Added [UCGUI Logger](Runtime/Components/Support/UCGUILogger.cs)
- Fixed a bug where buttons would not properly scale with their containing text when using `FitToContents()`
- Fixed the issue that the `VerticalLayoutGroup` wasn't targeted by the helper `ReverseArrangement()`
- Added some default styling options in [UCGUIStyles](Runtime/Components/Style/UCGUIStyles.cs)
- Made button implement `IEnabled`
- Buttons now won't automatically be created with an attached foreground and text component, minimizing object count

## [1.0.10] - 2025-09-14
### View(Stack) & Layout Components
- Further improvements to [Views](Runtime/Components/View.cs):
  - Renamed to `View` from `ViewComponent`. <i>This cange will probably come to more components in the future.</i>
  - The `onOpen` and `onClose` events are now only fired if explicitly initialized using `EventOpen` and `EventClose` respectively. Use `Events` to do both directly
  - Event creation behaviour can also be automatically added if setting `Default.View.AutoAddEvents` to `true`.
- New component: **[ViewStack](Runtime/Components/ViewStack.cs)**. This Stack manages your views in a, you guessed it, stack. This allows for some easier management of navigation flows and view coordination.
- New component: **[HStackComponent](Runtime/Components/HStackComponent.cs)**. This component aligns your items horizontally ;). Essentially an easier access for Unity's native `HorizontalLayoutGroup`.
- New component: **[VStackComponent](Runtime/Components/VStackComponent.cs)**. This component aligns your items vertically ;). Essentially an easier access for Unity's native `VerticalLayoutGroup`.
- New component: **[GridComponent](Runtime/Components/GridComponent.cs)**. This component aligns your items in a grid;). Essentially an easier access for Unity's native `GridLayoutGroup`.
- Introduction of UCGUI wide [Defaults](Runtime/Components/Support/Defaults.cs). These now also contain the previously separate `UCGUIStyles` under `Defaults.Styles`.
- Added native function to toggle focus state in `IFocusable`.
- Improved LayoutElement compatability

## [1.0.11] - 2025-10-03
### Better Inputs, Graphic Base & DebugMode
- Overhauled and improved [InputComponent](Runtime/Components/InputComponent.cs).
- Added new defaults
- Created a common ground for Graphic based components using [GraphicComponent](Runtime/Components/GraphicComponent.cs).
- Added new [DebugOptions](Runtime/Components/Support/DebugOptions.cs) and `DebugMode(DebugOptions)` for basic editor gizmos.
- Smaller reordering of code and comments for improved maintenance and legibility

## [1.0.12] - 2025-11-01
### Labels, TextAnimator, Layouts, DragViews & much more!
- Overhauled and improved [ButtonComponent](Runtime/Components/ButtonComponent.cs).
  - UCGUI's button now uses a [LabelComponent](Runtime/Components/LabelComponent.cs) for basic rendering, increasing the ease of use, whilst maintaining the option to be as flexible as before.
- Added [TextAnimator](Runtime/Components/Animation/TextAnimator.cs) for animations on texts.
  - Animations support three different modes: `Letter`, `Word` and `Sentence`.
- Added a new element into the layout element hierarchy: [SwitchLayoutComponent](Runtime/Components/SwitchLayoutComponent.cs).
- Any [LayoutComponent](Runtime/Components/LayoutComponent.cs) now disables its `ContentSizeFitter` if the size is set manually after creation.
- Improves stretching behaviour using new `StretchHorizontally`, `StretchVertically` and `Maximize` functionalities.
- Simplified the [DraggableViewComponent](Runtime/Components/DragViewComponent.cs) and the View family overall.
- Style presets now live inside their own classes, allowing for easier and more intuitive access.
- A new [ComponentFinder](Runtime/Service/ComponentFinder.cs) allows for a built-in global dictionary for components, as well as registering instances for classes.
- Removed the `Runtime/Demo` folder as any example functionality is being migrated to [the examples repository](https://github.com/GiorgioKalmund/UCGUI-Examples) 
  - Also removed other irrelevant scripts not core to the library

## [1.0.13] - 2025-11-12
### //FIXME :)
- Various fixes and improvements

## [1.0.14] - 2025-12-03
### Faster Layouts, Improved Views in Stacks & more
- The [Grid](Runtime/Components/GridComponent.cs) is now in feature parity with all the other layouts for various builder interactions.
- [Layouts](Runtime/Components/LayoutComponent.cs) no longer disable their graphic automatically. Instead, they change their color to .clear.
  - Horizontal and vertical layouts are also now marked for <b>immediate</b> rebuilding to reduce visual inconsistencies.
    - Might however change in the future due to the associated performance cost.
- Improved the [ViewComponent](Runtime/Components/ViewComponent.cs) by creating a common [Abstract Ancestor](Runtime/Components/AbstractViewComponent.cs).
  - Closing is now also split into a 'force' mode and a regular one, with the regular one additionally checking if the view is currently part of a [ViewStack](Runtime/Components/ViewStackComponent.cs). This change only allows views to be closed if they are at the top of a ViewStack (if they are part of one).
- The [SimpleScreen](Runtime/Components/Screen/SimpleScreen.cs) has new initializers, encouraging creation of the screen during the 'Awake' stage.
- [FocusState](Runtime/Components/Interface/IFocusable.cs)s can now be properly initialized and will initialize correctly if built during the 'Awake' stage.
- `UI.Image(...)` call now directly supports a string as a path to a texture, making use of the [ImageService](Runtime/Service/ImageService.cs)'s functionality even more.
- Constructors of all components are now protected, mirroring Unity's approach to MonoBehaviour instantiation.
- Fixed an issue where OffsetY would offset the element into the x direction.

## [1.0.15] - 2025-12-10
### Spacers
- Introduced a new [Spacer](Runtime/Components/SpacerComponent.cs) which automatically resizes inside a fixed layout to fill as much space as possible.
  - The Spacer as an associated `Defaults.Spacer.AlternateDirectionExtents` variable which determines its extents into the non-stretching direction.
- Added the `Press()` function for buttons to allow easier coordination of actions (i.e. between different input methods).
- Smaller adjustments