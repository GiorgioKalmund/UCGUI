using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UCGUI.Service;
using UCGUI.Support;

namespace UCGUI
{
    public partial class UI
    {
        #region Blank
        /// <summary>
        /// UCGUI's default Blank object. Will be sized 0x0.
        /// </summary>
        /// <param name="parent">Optional parent in the hierarchy.</param>
        /// <param name="name">Optional name for identification.</param>
        /// <returns>The resulting UCGUI <see cref="BaseComponent"/>.</returns>
        public static BaseComponent Blank(MonoBehaviour parent = null, string name = null)
        {
            BaseComponent empty = N<BaseComponent>();
            empty.Size(0, 0);
            empty.Parent(parent);
            if (name != null)
                empty.DisplayName = name;
            return empty;
        }
        #endregion

        #region Empty

        public static GameObject Empty(MonoBehaviour parent = null, string name = null) =>
            Empty(parent?.transform, false, name);

        #endregion
        
        #region Text 
        /// <summary>
        /// UCGUI's default Text Component.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="color">Optional <see cref="Color"/> of the font. Defaults to TextMeshPro's default.</param>
        /// <returns>The resulting UCGUI <see cref="TextComponent"/>. Use this to then continue building your desired Text Component.</returns>
        public static TextComponent Text(string text = null, Color? color = null)
        {
            TextComponent textComponent = N<TextComponent>();
            
            textComponent.Text(text);
            if (color.HasValue)
                textComponent.Color(color.Value);
            return textComponent;
        }

        /// <summary>
        /// Creates a new <see cref="TextComponent"/> from an integer value.
        /// </summary>
        /// <param name="text">The floating point value to be represented as text.</param>
        /// <param name="color">The (optional) color of the text.</param>
        /// <remarks>See more at <see cref="Text(string, Color?)"/></remarks>
        public static TextComponent Text(int text, Color? color = null) => Text(text.ToString(), color);

        /// <summary>
        /// Creates a new <see cref="TextComponent"/> from a floating point value.
        /// </summary>
        /// <param name="text">The floating point value to be represented as text.</param>
        /// <param name="format">Format of the floating point value. See remarks down below for more info.</param>
        /// <param name="color">The (optional) color of the text.</param>
        /// <remarks>
        /// See more at <see cref="Text(string, Color?)"/> <br></br> <br></br>
        /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.single.tostring?view=net-10.0"/> <br></br>
        /// <see href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings"/> <br></br>
        /// <see href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings"/> <br></br>
        /// </remarks>
        public static TextComponent Text(float text, string format = "0.00", Color? color = null) => Text(text.ToString(format), color);
        #endregion
        
        #region Image
        /// <summary>
        /// UCGUI's default Image Component.
        /// </summary>
        /// <param name="sprite">The sprite to display.</param>
        /// <param name="type">Optional <see cref="Image.Type"/> used to render the image. Defaults to <see cref="Image.Type.Simple"/>.</param>
        /// <param name="ppum">Optional value to determine the PixelsPerUnitMultiplier of the sprite. Relevant for example if you use <see cref="Image.Type.Sliced"/>.</param>
        /// <returns>The resulting UCGUI <see cref="ImageComponent"/>. Use this to then continue building your desired Image Component.</returns>
        public static ImageComponent Image([CanBeNull] Sprite sprite, Image.Type type = UnityEngine.UI.Image.Type.Simple,
            float ppum = -1f)
        {
            ImageComponent imageComponent = N<ImageComponent>();
            imageComponent.Sprite(sprite, type, ppum);
            return imageComponent;
        }

        public static ImageComponent Image(string pathName = null, Image.Type type = UnityEngine.UI.Image.Type.Simple,
            float ppum = -1f)
        {
            return Image(ImageService.GetSprite(pathName), type, ppum);
        }
        
        public static ImageComponent Image(Color color, float alpha)
        {
            ImageComponent imageComponent = N<ImageComponent>();
            imageComponent.Color(color, alpha);
            return imageComponent;
        }

        public static ImageComponent Image(Color color)
        {
            return Image(color, color.a);
        }
        #endregion

        #region Button
        public static ButtonComponent Button([CanBeNull] string text, UnityAction action = null,
            UnityAction<ButtonComponent.ButtonBuilder> label = null)
        {
            ButtonComponent buttonComponent = N<ButtonComponent>();
            buttonComponent.DisplayName($"[{text}]");

            if (text != null)
                buttonComponent.Text(text);
            if (action != null)
                buttonComponent.Function(action);
            if (label != null)
            {
                BaseComponent.BeginParentContext(buttonComponent);
                label(new ButtonComponent.ButtonBuilder(buttonComponent));
                BaseComponent.EndParentContext();
            }

            return buttonComponent;
        }

        public static ButtonComponent Button(UnityAction action = null,
            UnityAction<ButtonComponent.ButtonBuilder> label = null)
            => Button(null, action, label);
        #endregion

        #region Views
        #region ScrollView
        /// <summary>
        /// UCGUI's default Scroll View. Similar to UGUI's <see cref="ScrollRect"/>.
        /// </summary>
        /// <param name="direction"> The <see cref="ScrollViewDirection"/> in which scrolling is enabled.</param>
        /// <param name="spacing"> (Optional) The spacing between the elements. <i>Defaults to <b>0f</b></i>.</param>
        /// <param name="content"> The action wrapping the content of the scroll view.</param>
        /// <returns>
        /// The resulting UCGUI <see cref="ScrollViewComponent"/>
        /// </returns>
        public static ScrollViewComponent ScrollView(ScrollViewDirection direction, float spacing, UnityAction content)
        {
            ScrollViewComponent scrollViewComponent = N<ScrollViewComponent>();

            scrollViewComponent
                .ScrollingDirection(direction, true)
                .ContentSpacing(spacing);
            
            BaseComponent.BeginParentContext(scrollViewComponent.content);
            content();
            BaseComponent.EndParentContext();
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollViewComponent.content.GetRect());
            return scrollViewComponent;
        }
        
        /// <inheritdoc cref="ScrollView(UCGUI.ScrollViewDirection,float,UnityEngine.Events.UnityAction)"/>
        public static ScrollViewComponent ScrollView(ScrollViewDirection direction, UnityAction content)
        {
            return ScrollView(direction, 0f, content);
        }
        #endregion
        
        #region View
        /// <summary>
        /// UCGUI's default View. Can be opened and closed manually or using <see cref="InputAction"/>.
        /// </summary>
        /// <param name="canvas">The canvas the view attaches to. If set will <see cref="Maximize{T}"/> if no other <see cref="Size{T}(T,UnityEngine.Vector2)"/> is specified.</param>
        /// <param name="viewBuilder"><see cref="AbstractViewComponent.ViewBuilder"/> to add content and further customize the view.</param>
        /// <returns>
        /// The resulting UCGUI <see cref="AbstractViewComponent"/>.
        /// </returns>
        public static ViewComponent View(Canvas canvas, UnityAction<AbstractViewComponent.ViewBuilder> viewBuilder)
        {
            ViewComponent abstractViewComponent = N<ViewComponent>();
            
            BaseComponent.BeginParentContext(abstractViewComponent);
            viewBuilder(new AbstractViewComponent.ViewBuilder(abstractViewComponent, canvas));
            BaseComponent.EndParentContext();

            return abstractViewComponent;
        }

        /// <summary>
        /// UCGUI's default View. Can be opened and closed manually or using <see cref="InputAction"/>.
        /// </summary>
        /// <param name="viewBuilder"><see cref="AbstractViewComponent.ViewBuilder"/> to add content and further customize the view.</param>
        /// <returns>
        /// The resulting UCGUI <see cref="AbstractViewComponent"/>.
        /// </returns>
        public static ViewComponent View(UnityAction<AbstractViewComponent.ViewBuilder> viewBuilder) => View(null, viewBuilder);
        #endregion
        
        #region ViewStack
        /// <summary>
        /// A controlling component for opening and closing multiple <see cref="AbstractViewComponent"/>s.
        /// </summary>
        /// <param name="parent">(Optional) parent to directly attach to.</param>
        /// <returns>
        /// The resulting <see cref="ViewStackComponent"/>.
        /// </returns>
        public static ViewStackComponent ViewStack(Behaviour parent = null)
        {
            ViewStackComponent viewStackComponent = N<ViewStackComponent>();
            if (parent)
                viewStackComponent.Parent(parent);
            return viewStackComponent;
        }

        /// <inheritdoc cref="ViewStack(Behaviour)"/>
        public static ViewStackComponent ViewStack(Transform parent)
        {
            ViewStackComponent viewStackComponent = ViewStack();
            viewStackComponent.Parent(parent);
            return viewStackComponent;
        }
        #endregion
        #endregion
        
        #region Layout
        #region HStack
        /// <summary>
        /// A horizontal layout element, automatically resizing to its contents.
        /// </summary>
        /// <param name="spacing">
        /// Horizontal spacing between the elements.
        /// </param>
        /// <param name="childAlignment">
        /// Alignment of HStack children.
        /// </param>
        /// <param name="contents">
        /// <see cref="LayoutBuilder"/> to configure spacing and adding contents. 
        /// </param>
        /// <returns>
        /// The resulting HStack.
        /// <remarks>
        /// Under the hood it is simply an <see cref="ImageComponent"/>, allowing you to enable (<see cref="ImageComponent.ToggleVisibility()"/>), and then also directly modify the backdrop.
        /// </remarks>
        /// </returns>
        public static HStackComponent HStack(float spacing, TextAnchor childAlignment, UnityAction<LayoutBuilder> contents)
        {
            HStackComponent layout = N<HStackComponent>();
            layout.Spacing(spacing).ChildAlignment(childAlignment);
            var builder = new LayoutBuilder(layout, layout.HorizontalLayout);
            BaseComponent.BeginParentContext(layout);
            contents(builder);
            BaseComponent.EndParentContext();
            return layout;
        }
        /// <inheritdoc cref="HStack(float, TextAnchor, UnityAction{LayoutBuilder})"/>
        public static HStackComponent HStack(UnityAction<LayoutBuilder> contents)
            => HStack(0f, contents);
        /// <inheritdoc cref="HStack(float, TextAnchor, UnityAction{LayoutBuilder})"/>
        public static HStackComponent HStack(float spacing, UnityAction<LayoutBuilder> contents)
            => HStack(spacing, Defaults.Layout.StandardHorizontalChildAlignment, contents);
        /// <inheritdoc cref="HStack(float, TextAnchor, UnityAction{LayoutBuilder})"/>
        public static HStackComponent HStack(TextAnchor childAlignment, UnityAction<LayoutBuilder> contents)
            => HStack(0f, childAlignment, contents);
        #endregion

        #region VStack
        /// <summary>
        /// A vertical layout element, automatically resizing to its contents.
        /// </summary>
        /// <param name="spacing">
        /// Vertical spacing between the elements.
        /// </param>
        /// <param name="childAlignment">
        /// Alignment of VStack children.
        /// </param>
        /// <param name="contents">
        /// <see cref="LayoutBuilder"/> to configure spacing and adding contents. 
        /// </param>
        /// <returns>
        /// The resulting VStack.
        /// <remarks>
        /// Under the hood it is simply an <see cref="ImageComponent"/>, allowing you to enable (<see cref="ImageComponent.ToggleVisibility()"/>), and then also directly modify the backdrop.
        /// </remarks>
        /// </returns>
        public static VStackComponent VStack(float spacing, TextAnchor childAlignment, UnityAction<LayoutBuilder> contents)
        {
            VStackComponent layout = N<VStackComponent>();
            layout.Spacing(spacing).ChildAlignment(childAlignment);
            var builder = new LayoutBuilder(layout, layout.VerticalLayout);
            BaseComponent.BeginParentContext(layout);
            contents(builder);
            BaseComponent.EndParentContext();
            return layout;
        }
        /// <inheritdoc cref="VStack(float, TextAnchor, UnityAction{LayoutBuilder})"/>
        public static VStackComponent VStack(UnityAction<LayoutBuilder> contents)
            => VStack(0f, contents);
        /// <inheritdoc cref="VStack(float, TextAnchor, UnityAction{LayoutBuilder})"/>
        public static VStackComponent VStack(float spacing, UnityAction<LayoutBuilder> contents)
            => VStack(spacing, Defaults.Layout.StandardVerticalChildAlignment, contents);
        /// <inheritdoc cref="VStack(float, TextAnchor, UnityAction{LayoutBuilder})"/>
        public static VStackComponent VStack(TextAnchor childAlignment, UnityAction<LayoutBuilder> contents)
            => VStack(0f, childAlignment, contents);
        #endregion

        #region ZStack
        /// <summary>
        /// Syntactic sugar for <see cref="BaseComponent.ParentContext(BaseComponent, UnityAction)"/>, using an <see cref="Blank"/> object.
        /// </summary>
        public static void ZStack(UnityAction content) => ZStack(Blank().DisplayName("ZStack"), content);
        /// <summary>
        /// Syntactic sugar for <see cref="BaseComponent.ParentContext(BaseComponent, UnityAction)"/>.
        /// </summary>
        public static void ZStack(BaseComponent parent, UnityAction content) => ZStack(parent.transform, content);
        /// <summary>
        /// Syntactic sugar for <see cref="BaseComponent.ParentContext(GameObject, UnityAction)"/>.
        /// </summary>
        public static void ZStack(GameObject    parent, UnityAction content) => ZStack(parent.transform, content);
        /// <summary>
        /// Syntactic sugar for <see cref="BaseComponent.ParentContext(Transform, UnityAction)"/>.
        /// </summary>
        public static void ZStack(Transform parent, UnityAction content)
        {
            BaseComponent.ParentContext(parent, content);
        }
        #endregion

        #region Spacer
        /// <summary>
        /// A greedy layout element which takes as much space as it can according to its specified <see cref="ISpacerBehaviour"/>.
        /// </summary>
        /// <returns>The resulting <see cref="SpacerComponent"/>.</returns>
        public static SpacerComponent Spacer()
        {
            SpacerComponent spacer = N<SpacerComponent>();
            return spacer;
        }

        
        /// <summary>
        /// A greedy layout element which takes as much space as it can according to its specified <see cref="ISpacerBehaviour"/>.
        /// </summary>
        /// <param name="behaviour">Custom <see cref="ISpacerBehaviour"/> which the spacer should use.</param>
        /// <returns>The resulting <see cref="SpacerComponent"/>.</returns>
        public static SpacerComponent Spacer(ISpacerBehaviour behaviour)
        {
            SpacerComponent spacer = N<SpacerComponent>();
            spacer.SetBehaviour(behaviour);
            return spacer;
        }
        #endregion

        #region Grid
        /// <summary>
        /// A grid layout element based on Unity's <see cref="GridLayoutGroup"/>.
        /// </summary>
        /// <param name="constraint"><see cref="GridLayoutGroup.Constraint"/> for the layout.</param>
        /// <param name="constraintCount">The count for the given constraint (if applicable).</param>
        /// <param name="childAlignment">The alignment of the grid's children within it.</param>
        /// <param name="grid"><see cref="GridComponent.GridBuilder"/> to configure the grid further.</param>
        /// <returns>
        /// The resulting <see cref="GridComponent"/>.
        /// </returns>
        public static GridComponent Grid(GridLayoutGroup.Constraint constraint, int constraintCount, TextAnchor childAlignment, UnityAction<GridComponent.GridBuilder> grid)
        {
            GridComponent gridComponent = N<GridComponent>();
            var builder = new GridComponent.GridBuilder(gridComponent);
            builder.GetGrid().constraint = constraint;
            builder.GetGrid().constraintCount = constraintCount;
            builder.GetGrid().childAlignment = childAlignment;
            BaseComponent.BeginParentContext(gridComponent);
            grid(builder);
            BaseComponent.EndParentContext();
            return gridComponent;
        }

        /// <summary>
        /// A grid layout element based on Unity's <see cref="GridLayoutGroup"/>.
        /// </summary>
        /// <param name="constraint"><see cref="GridLayoutGroup.Constraint"/> for the layout.</param>
        /// <param name="constraintCount">The count for the given constraint (if applicable).</param>
        /// <param name="grid"><see cref="GridComponent.GridBuilder"/> to configure the grid further.</param>
        /// <returns>
        /// The resulting <see cref="GridComponent"/>.
        /// </returns>
        public static GridComponent Grid(GridLayoutGroup.Constraint constraint, int constraintCount,
            UnityAction<GridComponent.GridBuilder> grid)
        {
            return Grid(constraint, constraintCount, TextAnchor.MiddleCenter, grid);
        }

        /// <summary>
        /// A grid layout element based on Unity's <see cref="GridLayoutGroup"/>.
        /// </summary>
        /// <param name="grid"><see cref="GridComponent.GridBuilder"/> to configure the grid further.</param>
        /// <returns>
        /// The resulting <see cref="GridComponent"/>.
        /// </returns>
        public static GridComponent Grid(UnityAction<GridComponent.GridBuilder> grid) => Grid(GridLayoutGroup.Constraint.Flexible, 0, grid);
        
        /// <summary>
        /// A grid layout element based on Unity's <see cref="GridLayoutGroup"/>.
        /// </summary>
        /// <param name="childAlignment">The alignment of the grid's children within it.</param>
        /// <param name="grid"><see cref="GridComponent.GridBuilder"/> to configure the grid further.</param>
        /// <returns>
        /// The resulting <see cref="GridComponent"/>.
        /// </returns>
        public static GridComponent Grid(TextAnchor childAlignment, UnityAction<GridComponent.GridBuilder> grid) => Grid(GridLayoutGroup.Constraint.Flexible, 0, childAlignment, grid);
        #endregion
        #endregion

        /// <summary>
        /// An input / text field based on <see cref="TMP_InputField"/>.
        /// </summary>
        /// <param name="placeholder">Placeholder string for the input field.</param>
        /// <param name="contentType"><see cref="TMP_InputField.ContentType"/> for the input field.</param>
        /// <param name="builder">(Optional) <see cref="InputComponent.InputBuilder"/> to configure the input further.</param>
        /// <returns>
        /// The resulting <see cref="InputComponent"/>.
        /// </returns>
        public static InputComponent Input(string placeholder, TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard, [CanBeNull] UnityAction<InputComponent.InputBuilder> builder = null)
        {
            InputComponent inputComponent = N<InputComponent>();
            inputComponent.Placeholder(placeholder);
            inputComponent.ContentType(contentType);
            if (builder != null)
                builder(new InputComponent.InputBuilder(inputComponent));

            return inputComponent;
        }
        
        /// <summary>
        /// An input / text field based on <see cref="TMP_InputField"/>.
        /// </summary>
        /// <param name="placeholder">Placeholder string for the input field.</param>
        /// <param name="builder"><see cref="InputComponent.InputBuilder"/> to configure the input further.</param>
        /// <returns>
        /// The resulting <see cref="InputComponent"/>.
        /// </returns>
        public static InputComponent Input(string placeholder, UnityAction<InputComponent.InputBuilder> builder) =>
            Input(placeholder, TMP_InputField.ContentType.Standard, builder);
        
        /// <summary>
        /// UCGUI's default Slider Component. Emulates Unity's uGUI native slider element.
        /// </summary>
        /// <param name="range">
        /// The <see cref="Range"/> the of values the slider covers, inclusive. Min has to be larger than Max, otherwise they will be automatically flipped.
        /// </param>
        /// <param name="builder">
        /// <see cref="SliderComponent.SliderBuilder"/> with most of the commonly used functions used to configure your slider.
        /// </param>
        /// <param name="onValueChanged">
        /// Callback invoked every time the slider's value changes.
        /// </param>
        /// <returns>
        /// The resulting UCGUI <see cref="SliderComponent"/>
        /// </returns>
        /// <example>
        /// <code>
        /// UCGUI.Slider(new Range(0, 1), builder =>
        /// {
        ///    builder.Foreground(healthbarFull);
        ///    builder.Background(healthbarEmpty);
        ///    builder.Handle(handleSprite);
        ///        
        /// },  newValue =>
        /// { 
        ///     Debug.Log("Slider Value Changed! " + newValue);
        /// }).Parent(canvas);
        /// </code>
        /// </example>.
        public static SliderComponent Slider(Range range, UnityAction<float> onValueChanged = null)
        {
            SliderComponent slider = N<SliderComponent>();
            slider.SetRange(range);

            if (onValueChanged != null)
                slider.OnValueChanged.AddListener(onValueChanged);

            return slider;
        }

        /// <summary>
        /// Creates a <see cref="LabelComponent"/>.
        /// </summary>
        /// <param name="text">The text of the label.</param>
        /// <param name="image">The optional image of the label as a <see cref="Sprite"/>.</param>
        /// <returns>
        /// The resulting <see cref="LabelComponent"/>.
        /// </returns>
        public static LabelComponent Label(string text, Sprite image)
        {
            LabelComponent label = N<LabelComponent>();
            label.Init(text, image);
            
            return label;
        }
        
        /// <summary>
        /// Creates a <see cref="LabelComponent"/>.
        /// </summary>
        /// <param name="text">The text of the label.</param>
        /// <param name="texture">The optional image of the label as a <see cref="Texture2D"/>.</param>
        /// <returns>
        /// The resulting <see cref="LabelComponent"/>.
        /// </returns>
        public static LabelComponent Label(string text, Texture2D texture) => Label(text, texture?.ToSprite());
        
        /// <summary>
        /// Creates a <see cref="LabelComponent"/>.
        /// </summary>
        /// <param name="text">The text of the label.</param>
        /// <returns>
        /// The resulting <see cref="LabelComponent"/>.
        /// </returns>
        public static LabelComponent Label(string text) => Label(text, (Sprite)null);
    }
}