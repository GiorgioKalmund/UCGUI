# UCGUI - Unity Codable GUI
A code-based graphics library for the Unity Game Engine

### Why UCGUI?

Simple UI's can easily be created in the editor, however they often need assistance from scripts and other libraries to do things like animation, transitions, etc.
UCGUI can be seen as a code-based superset of [Unity's UGUI](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/index.html). Built in tools allow easy and accesible creation of [Text](Runtime/Components/TextComponent.cs), [Images](Runtime/Components/ImageComponent.cs), [Buttons](Runtime/Components/ButtonComponent.cs), [Textfields](Runtime/Components/InputComponent.cs), [Sliders](Runtime/Components/SliderComponent.cs), [Popups](Runtime/Components/PopupComponent.cs), [Windows](Runtime/Components/Window/WindowComponent.cs) and so much more. Everything is based on a singular [BaseComponent]() class and every class is created in such a way that it can be easily inherited from and expanded upon.
<br><br> <i>It might not be for everyone, but it help me at least to keep everything **inside** my code, as code, without having to constantly search through the hierarchy to edit one specific parameter of my UI. Everything is just code, and way more flexible as it is directly references during runtime.</i>

### Installation

UCGUI is available on [GitHub](https://github.com/GiorgioKalmund/UCGUI) and can be installed via the **Unity Package Manager**.

1. Open the Unity Package Manager (`Window ▶ Package Management ▶ Package Manager`)
2. In the upper left corner, press on the `+`symbol and select `Install package from git URL`
3. Paste the link and click `Add`

```
https://github.com/GiorgioKalmund/UCGUI.git
```

This will add the latest version of UCGUI to your Unity Project!

### Usage and Examples