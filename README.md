# UCGUI - Unity Codable GUI
<div align="center">
<img width="64" height="64" alt="ucgui-logo" src="https://github.com/user-attachments/assets/0bc8274d-021e-4d73-a094-f08b95e2559c"/>
    
[![Unity](https://img.shields.io/badge/Unity-black.svg?style=for-the-badge&logo=unity)](http://www.unity.com)
</div>
A code-based graphics library for the Unity Game Engine

## Why UCGUI?

Simple UIs can easily be created in the editor, however they often need assistance from scripts and other libraries to do things like animation, transitions, etc.
This is where UCGUI jumps in, which can be seen as a code-based superset of [Unity's built in uGUI](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/index.html).<br> UCGUI's built in tools allow easy and accessible creation of [Text](Runtime/Components/TextComponent.cs), [Images](Runtime/Components/ImageComponent.cs), [Buttons](Runtime/Components/ButtonComponent.cs), [Textfields](Runtime/Components/InputComponent.cs), [Sliders](Runtime/Components/SliderComponent.cs), [Views](Runtime/Components/ViewComponent.cs), [Windows](Runtime/Components/Window/WindowComponent.cs) and so much more. Everything is based on a singular [BaseComponent]() class and every class is created in such a way that it can be easily inherited from and expanded upon.
<br><br>The main benefit comes from inherent control over your UI at runtime, allowing dynamic UI combinations to be created faster and frictionless.
<br><br> <i>It might not be for everyone, but it helps me at least to keep everything **inside** my code, as code, without having to constantly search through the hierarchy to edit one specific parameter of my UI. Everything is just code, and way more flexible as it can be easily referenced during runtime.</i>

## Installation

UCGUI is available on [GitHub](https://github.com/GiorgioKalmund/UCGUI) and can be installed via the **Unity Package Manager**.

1. Open the Unity Package Manager (`Window ▶ Package Management ▶ Package Manager`)
2. In the upper left corner, press on the `+` symbol and select `Install package from git URL`
3. Paste the link and click `Add`

```
https://github.com/GiorgioKalmund/UCGUI.git
```

This will add the latest version of UCGUI to your Unity project.

## Usage, Patterns, Examples

UCGUI offers you two main ways of creating UI components. Let's look at some simple examples on how to create UCGUI's components using both of them.

### [UI](Runtime/Components/UI.cs)
This class offers simple but flexible builders for all of UCGUI's base components.

### Text
Creating a text on your screen which sticks to the top left of the canvas with some offset could look like this:
```csharp
// Returns the resulting TextComponent
UI.Text("Hello, World!")
    .Pivot(PivotPosition.UpperLeft, true) // Sets the pivot point. Additionally adding true, will also achor it to that same corner inside of the parent
    .Offset(50, -50) // Offsets the position by 50 on the x, and -50 on the y-axis. Relative to the pivot of the RectTransform
    .FitToContents() // Makes the RectTransform adjust its size to the preferred size of the text
    .Parent(canvas); // Finally, parents the GameObject to the canvas
```
As we are using a flexible builder pattern, everything addition is optional, even the `.Parent()`.
<br> Under the hood, the [UI class](Runtime/Components/UI.cs) simply uses the `UI` to create its elements.
<br> Feel free to take a look a how these supporting builders are made, so you can also add your own patterns if you want.

### Button
```csharp
// Returns the reslting ButtonComponent
UI.Button("MyButton", action =>
{
    // The function being executed when the button is pressed
    MyFunc();
    
}, label => 
{
    // Visual customization 
    label.Background(_buttonBackgroundSprite);
    label.Foreground(_foregroundSprite);
    label.FitToContents();
    label.Padding(new RectOffset(20, 20, 10, 10));
})
.Parent(parent);
```
This creates a [Button](Runtime/Components/ButtonComponent.cs), which under the principally works just like any other regular uGUI Button. However, it also offers out of the box flexibility for content based resizing in both the horizontal and vertical direction, as well as
an optional `Foreground` sprite which lives between the button's background and text. The `label` in the example above is a simple `ButtonBuilder`, which allows you to get an overview of all the available functions for a simple but well-designed button.
For example, `Padding()` allows you to specify optional padding on all the four sides between the text and the borders of the button. It is most commonly used in combination with `FitToContents`.
<br> Again, the entire `label` block is optional, however `action` is not when using this pattern builder.

### [UI (Base)](Runtime/Components/Support/UI.cs)
This file acts as the second half of the `UI` class and serves as a foundation for creating components in UCGUI. It contains all the builder pattern styling functions applicable to **every** [BaseComponent](Runtime/Components/BaseComponent.cs).
If you have custom components, they will be instantiated via this class.
<br> Naturally, all of UCGUI's default components can be instantiated in this way too:
```csharp
// Returns the resulting ImageComponent
ImageComponent myImage = UI.N<ImageComponent>("GameObject name", parent)
    .Sprite(_teapotSprite)
    .Alpha(0.5f)
    .Color(Color.red)
    .Size(300, 300)
```
This code would create a 300x300 image, located at the center of the parent, with sprite, color and alpha applied.
[N\<T\>](Runtime/Components/Support/UI.cs) (<i>"New"</i>) is the generic method used to create any component.

```csharp
// The TextComponentExample from above, just in another format now    
// Returns the resulting TextComponent 
TextComponent myText = UI.N<TextComponent>("GameObject2 name", parent)
    .Text("Hello, World!")
    .Pivot(PivotPosition.UpperLeft, true)
    .Offset(50, -50)
    .FitToContents()
    .Parent(canvas); 
```
This code produces an identical [Text](Runtime/Components/TextComponent.cs) to the one from the example above, we are just using a different syntax :)

---

### Views / ViewStack
[Views](Runtime/Components/ViewComponent.cs) are one of two ways of managing your UI. Using the [ViewBuilder](Runtime/Components/ViewComponent.cs) you can easily create an area for you content. Views are very versatile. 
They easily scale to fit their entire canvas parent and can be configured to close on a background tap or on an InputAction.
<br>
Views themselves are ideally just parent container for any other component, however they inherit from [ImageComponent](Runtime/Components/ImageComponent.cs) for faster configuration of the backdrop.
<br>
<br>
Here is a small example:
```csharp
ViewStack stack = UI.ViewStack(canvas);
    
View view1 = UI.View(canvas, v =>
{
    v.Add(UI.Text("1"));
    v.CloseOnBackgroundTap();
}).Size(800, 800); // Overrides the default canvas-filling behaviour of the view.

View view2 = UI.View(canvas, v =>
{
    v.Add(UI.Text("2"));
});

// more views...

------------------------------
// Some interaction might then call
stack.Push(view1);
// and later
stack.Push(view2);
// ...
stack.Push(view3);
------------------------------

------------------------------
// Afterwards we can:
stack.Pop(); // Removes and closes last pushed view
stack.PopUntil(view1); // Does what it says ;)
stack.Collapse(); // Pops all remaining views
------------------------------
```
In our example we would now have revealed two views with `view2` always displaying om top of `view1`, no matter the creation order.<br> We can see we can easily create a view stack and directly create our own navigation flow!
Using `Pop()` we can then move backwards in the stack, revealing the underlying views one by one.
<br><br>
ViewStacks are quite versatile, also allowing an immediate dissolving of the stack as well as going back to a specific view. 

### Screens

Screens are the bridge between the editor and UCGUI. 
A screen acts as your canvas for creating your UI and components. They are an abstract class and enforce two methods. `Setup()`: this is where you will build your UI. It is called in the `Start()` phase of [Unity's Event Loop](https://docs.unity3d.com/6000.2/Documentation/Manual/execution-order.html).
`Canvas GetCanvas()` expects you to return the canvas the screen should attach to.
<br> Let's look at a simple example:
```csharp
using UCGUI;

public class MyScreen : SimpleScreen
{
    public void Setup()
    {
        // Every screen has a direct reference to the Canvas (`canvas`) it is attached to,
        // allowing quick access for `Parent()`
        var title = UI.Text("Title").Parent(canvas);
        
        var image = ...
        
        // ... Create your UI here
    }
    
    public Canvas GetCanvas()
    {
        return GetComponentInParent<Canvas>();
    }
}
```

<i><br> Of course you can manually add any component to GameObjects directly, however screens exist for a reason... ;)</i>

----

## Interfaces

UCGUI adds multiple interfaces which can come in handy in certain situations.
### [IFocusable](Runtime/Components/Interface/IFocusable.cs)

This interface introduces the concept of **focus** in UCGUI. Any script implementing it will have to implement three members:
<br> `HandleFocus()` and `HandleUnfocus()`, as well as `int GetFocusGroup()`. The former two are invoked every time the object is focused via `Focus()`, or when it has lost focus.
`Unfocus()`. To understand their automated behaviour we have to first understand `FocusGroups`.
<br>
<br> The concept of `FocusGroups` allows multiple objects to synchronize their focus state automatically.
<br> For example, I have three buttons, all part of in group `1`. Whenever I call `Focus()` on any one of them, the previously focused button will automatically call `Unfocus`.
Thus, per group, only one element at a time will be the focused element, whilst all other ones aren't. 
<br><i>By default, all elements implementing IFocusable are part of group 0.</i>

### [ICopyable\<T\>](Runtime/Components/Interface/ICopyable.cs) 
As copying GameObjects in Unity sadly isn't always as simple as calling `Instantiate()`, this interfaces aims to bridge this gap.
Any member implementing this interface has to implement two functions:
`Copy()` and `CopyFrom()`.
<br> Even there are no clear guidelines how these functions should work they are expected to do the following:
#### Copy()
```csharp
// Create a base element
var originalText = UI.Text("Hello, this is my original Text").Color(Color.red).Parent(canvas);
// Create a new copy of that that element using Copy()
var copyText = originalText.Copy().Offset(0, -50).Color(Color.blue);
```
In this example, `copyText` will also read `"Hello, this is my original Text"`, as well as being also parented to the `canvas` object.
Additionally, it will also be offset by -50 on the y-axis and appear blue instead of red.

#### CopyFrom(other)
```csharp
// Create a base element
var originalHorizontalLayoutGroup = ... ;
// Create a button    
var myButton = ... ;

// Now only 'mirror' the HorizontalLayoutGroup over to the button
myButton.HorizontalLayout.CopyFrom(originalHorizontalLayoutGroup);
```
This function offers a more fine-grained control over what exactly is copied. For example, a single `CopyFrom` call of a larger, more complex component, will often be
composed of multiple `CopyFrom` calls for all of its individual components.

Most of UCGUI's default components implement this functionality. For a non-trivial reference take a look at the [Button](Runtime/Components/ButtonComponent.cs).

### [IRenderable](Runtime/Components/Interface/IRenderable.cs)
This interface simply enforces the function `Render()`. This function should optimally be [idempotent](https://en.wikipedia.org/wiki/Idempotence#:~:text=is%20the%20property%20of%20certain%20operations%20in%20mathematics%20and%20computer%20science%20whereby%20they%20can%20be%20applied%20multiple%20times%20without%20changing%20the%20result%20beyond%20the%20initial%20application.). This can be used to easily re-render hierarchies recursively.

### [IEnabled](Runtime/Components/Interface/IEnabled.cs)
This interface can be used to enforce uniform access to an `Enable()` call, allowing faster and easier bulk enabling and disabling of components.

### [IStylable](Runtime/Components/Interface/IStylable.cs)
IStylable aims to provide a quick and easy way to style your buttons throughout your project. <br>
In combination with [Styles](Runtime/Components/Style/AbstractStyle.cs) you can very quickly bundle and re-use your configurations for a variety of built-in components,
as well as allowing you to expand the concept to any of your custom ones.
<br><br>
Let's look at a small example:
```csharp
// Create your project's default style
public static ButtonStyle Default = new ButtonStyle(btn =>
{
    btn.FitToContents(20, 20)
        .Foreground(defaultIcon)
        .SpriteSwap(highlightedSprite, pressedSprite, disabledSprite)
        .Sprite(buttonSprite)
        ;
});

// Create a variant using 'Expand'
public static ButtonStyle DefaultRed = Default.Expand(btn =>
{
    btn.Color(Color.red);
});
```
As we might want a uniform button design across our game we can create a default [button style](Runtime/Components/Style/ButtonStyle.cs) for our project.
<br>
This allows us to reuse this style on any button without having to respecify all of our desired formatting options every time. Define it once
and that's it.
<br><br>
We can expand on this concept by using `Expand` (*badum-ts*). Expand allows us to build on top of existing styles, creating some form of hierarchical structure within your styles.
In our example from above, any button styles using 'DefaultRed' would also have all the previous formatting options applied by 'Default', whilst additionally also making the button red.
<br><br>
Applying our new styles to a button is now only a single call to `Style` (*i.e. If the component is implementing IStylable*):
```csharp
UI.Button("Hello, World!", () =>
{
    Debug.Log("Pressed!");
}, label =>
{

}).Parent(canvas)
.Style(DefaultRed); // Apply our custom style
```
These styles are also exist for other built-in components such as the [TextStyle](Runtime/Components/Style/TextStyle.cs) and more.

*UCGUI comes with some preset styles for quick prototyping which can be found in [UCGUIStyles](Runtime/Components/Style/UCGUIStyles.cs)*.
