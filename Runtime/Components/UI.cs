using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UCGUI.Services;

namespace UCGUI
{
    public partial class UI
    {
        /// <summary>
        /// UCGUI's default Text Component.
        /// </summary>
        /// <param name="text">
        /// The text to display.
        /// </param>
        /// <param name="color">
        /// Optional <see cref="Color"/> of the font. Defaults to TextMeshPro's default.
        /// </param>
        /// <returns>
        /// The resulting UCGUI <see cref="TextComponent"/>. Use this to then continue building your desired Text Component.
        /// </returns>
        public static TextComponent Text(string text = null, Color? color = null)
        {
            TextComponent textComponent = N<TextComponent>();
            textComponent.Text(text);
            if (color.HasValue)
                textComponent.Color(color.Value);
            return textComponent;
        }

        /// <summary>
        /// UCGUI's default Image Component.
        /// </summary>
        /// <param name="sprite">
        /// The sprite to display.
        /// </param>
        /// <param name="type">
        /// Optional <see cref="Image.Type"/> used to render the image. Defaults to <see cref="Image.Type.Simple"/>.
        /// </param>
        /// <param name="ppum">
        /// Optional value to determine the PixelsPerUnitMultiplier of the sprite. Relevant for example if you use <see cref="Image.Type.Sliced"/>.
        /// </param>
        /// <returns>
        /// The resulting UCGUI <see cref="ImageComponent"/>. Use this to then continue building your desired Image Component.
        /// </returns>
        public static ImageComponent Image([CanBeNull] Sprite sprite, Image.Type type = UnityEngine.UI.Image.Type.Simple,
            float ppum = 1f)
        {
            ImageComponent imageComponent = N<ImageComponent>();
            imageComponent.Sprite(sprite, type, ppum);
            return imageComponent;
        }

        public static ImageComponent Image(string pathName = null, Image.Type type = UnityEngine.UI.Image.Type.Simple,
            float ppum = 1f)
        {
            return Image(ImageService.GetSprite(pathName), type, ppum);
        }
        
        public static ImageComponent Image(Color color, float alpha = 1f)
        {
            ImageComponent imageComponent = N<ImageComponent>();
            imageComponent.Color(color, alpha);
            return imageComponent;
        }

        /// <summary>
        /// UCGUI's default Button Component. Similar to Unity's UGUI's <see cref="UnityEngine.UI.Button"/>, however more flexible out of the box.
        /// </summary>
        /// <param name="text">
        /// (Optional) text to be displayed on the button.
        /// </param>
        /// <param name="action">
        /// The action being executed when pressing the button.
        /// </param>
        /// <param name="label">
        /// Label <see cref="ButtonComponent.ButtonBuilder"/> to configure the visuals and formatting behaviour.
        /// </param>
        /// <returns>
        /// The resulting UCGUI <see cref="ButtonComponent"/>.
        /// </returns>
        /// <example>
        /// <code>
        /// 
        /// UCGUI.Button("Press me!", () =>
        /// {
        ///     Debug.Log("Button pressed!");
        /// }, label =>
        /// {
        ///     label.TextStyle(Color.white, FontStyles.Bold | FontStyles.Italic);
        ///     label.FitToContents(spacing:20);
        ///     label.Padding(new RectOffset(20, 20, 20, 20));
        ///     label.Foreground(foreground_image);
        ///     label.Background(Color.gray);
        /// }).Parent(canvas);
        /// </code>
        /// </example>

        public static ButtonComponent Button([CanBeNull] string text, UnityAction action = null,
            UnityAction<ButtonComponent.ButtonBuilder> label = null)
        {
            ButtonComponent buttonComponent = N<ButtonComponent>();

            if (text != null)
                buttonComponent.Text(text);
            if (action != null)
                buttonComponent.Function(action);
            if (label != null)
            {
                label(new ButtonComponent.ButtonBuilder(buttonComponent));
            }

            return buttonComponent;
        }

        public static ButtonComponent Button(UnityAction action = null,
            UnityAction<ButtonComponent.ButtonBuilder> label = null)
            => Button(null, action, label);

        /// <summary>
        /// UCGUI's default Scroll View. Similar to UGUI's <see cref="ScrollRect"/>.
        /// </summary>
        /// <param name="direction"> The <see cref="ScrollViewDirection"/> in which scrolling is enabled.</param>
        /// <param name="spacing"> (Optional) The spacing between the elements. <i>Defaults to <b>0f</b></i>.</param>
        /// <param name="alignment"> (Optional) the alignment of the children within the scroll view. <i>Defaults to <see cref="TextAnchor.UpperCenter"/></i>.</param>
        /// <param name="content"> <see cref="ScrollViewComponent.ScrollViewBuilder"/> to add content and further customize the scroll view. Will default to pivoting around and anchoring to <see cref="PivotPosition.UpperCenter"/> if the scroll view is vertical, else to <see cref="PivotPosition.MiddleLeft"/>.</param>
        /// <returns>
        /// The resulting UCGUI <see cref="ScrollViewComponent"/>
        /// </returns>
        public static ScrollViewComponent ScrollView(ScrollViewDirection direction, float spacing, TextAnchor alignment, UnityAction<ScrollViewComponent.ScrollViewBuilder> content)
        {
            ScrollViewComponent scrollViewComponent = N<ScrollViewComponent>();

            scrollViewComponent.ScrollingDirection(direction);
            scrollViewComponent.content
                .AddLayout(direction, spacing, alignment)
                .AddFitter(direction)
                .Pivot(direction == ScrollViewDirection.Vertical ? PivotPosition.UpperCenter : PivotPosition.MiddleLeft, true);
            
            content(new ScrollViewComponent.ScrollViewBuilder(scrollViewComponent));
            return scrollViewComponent;
        }

        /// <inheritdoc cref="ScrollView(UCGUI.ScrollViewDirection,float,UnityEngine.TextAnchor,UnityEngine.Events.UnityAction{UCGUI.ScrollViewComponent.ScrollViewBuilder})"/>
        public static ScrollViewComponent ScrollView(ScrollViewDirection direction, float spacing, UnityAction<ScrollViewComponent.ScrollViewBuilder> content)
        {
            return ScrollView(direction, spacing, direction == ScrollViewDirection.Vertical ? TextAnchor.UpperCenter : TextAnchor.MiddleLeft, content);
        }
        /// <inheritdoc cref="ScrollView(UCGUI.ScrollViewDirection,float,UnityEngine.TextAnchor,UnityEngine.Events.UnityAction{UCGUI.ScrollViewComponent.ScrollViewBuilder})"/>
        public static ScrollViewComponent ScrollView(ScrollViewDirection direction, UnityAction<ScrollViewComponent.ScrollViewBuilder> content)
        {
            return ScrollView(direction, 0f, content);
        }

        /// <summary>
        /// UCGUI's default View. Can be opened and closed manually or using <see cref="InputAction"/>.
        /// </summary>
        /// <param name="canvas">The canvas the view attaches to. If set will <see cref="FullScreen{T}"/> if no other <see cref="Size{T}(T,UnityEngine.Vector2)"/> is specified.</param>
        /// <param name="viewBuilder"><see cref="AbstractViewComponent.ViewBuilder"/> to add content and further customize the view.</param>
        /// <returns>
        /// The resulting UCGUI <see cref="AbstractViewComponent"/>.
        /// </returns>
        public static ViewComponent View(Canvas canvas, UnityAction<AbstractViewComponent.ViewBuilder> viewBuilder)
        {
            ViewComponent abstractViewComponent = N<ViewComponent>();
            
            viewBuilder(new AbstractViewComponent.ViewBuilder(abstractViewComponent, canvas));

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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="viewBuilder"></param>
        /// <returns></returns>
        public static DragViewComponent DragView(Canvas canvas, UnityAction<DragViewComponent.DragViewBuilder> viewBuilder)
        {
            DragViewComponent dragViewComponent = N<DragViewComponent>();
            
            viewBuilder(new DragViewComponent.DragViewBuilder(dragViewComponent, canvas));

            return dragViewComponent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dragViewBuilder"></param>
        /// <returns></returns>
        public static DragViewComponent DragView(UnityAction<DragViewComponent.DragViewBuilder> dragViewBuilder) =>
            DragView(null, dragViewBuilder);
        
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
        public static HStackComponent HStack(float spacing, TextAnchor childAlignment, Action<LayoutBuilder> contents)
        {
            HStackComponent layout = N<HStackComponent>();
            layout.Spacing(spacing).ChildAlignment(childAlignment);
            var builder = new LayoutBuilder(layout, layout.HorizontalLayout);
            contents(builder);
            return layout;
        }
        /// <inheritdoc cref="HStack(float, TextAnchor, Action{LayoutBuilder})"/>
        public static HStackComponent HStack(Action<LayoutBuilder> contents)
            => HStack(0f, contents);
        /// <inheritdoc cref="HStack(float, TextAnchor, Action{LayoutBuilder})"/>
        public static HStackComponent HStack(float spacing, Action<LayoutBuilder> contents)
            => HStack(spacing, TextAnchor.UpperCenter, contents);
        /// <inheritdoc cref="HStack(float, TextAnchor, Action{LayoutBuilder})"/>
        public static HStackComponent HStack(TextAnchor childAlignment, Action<LayoutBuilder> contents)
            => HStack(0f, childAlignment, contents);
        
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
        public static VStackComponent VStack(float spacing, TextAnchor childAlignment, Action<LayoutBuilder> contents)
        {
            VStackComponent layout = N<VStackComponent>();
            layout.Spacing(spacing).ChildAlignment(childAlignment);
            var builder = new LayoutBuilder(layout, layout.VerticalLayout);
            contents(builder);
            return layout;
        }
        /// <inheritdoc cref="VStack(float, TextAnchor, Action{LayoutBuilder})"/>
        public static VStackComponent VStack(Action<LayoutBuilder> contents)
            => VStack(0f, contents);
        /// <inheritdoc cref="VStack(float, TextAnchor, Action{LayoutBuilder})"/>
        public static VStackComponent VStack(float spacing, Action<LayoutBuilder> contents)
            => VStack(spacing, TextAnchor.MiddleLeft, contents);
        /// <inheritdoc cref="VStack(float, TextAnchor, Action{LayoutBuilder})"/>
        public static VStackComponent VStack(TextAnchor childAlignment, Action<LayoutBuilder> contents)
            => VStack(0f, childAlignment, contents);

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
        public static GridComponent Grid(GridLayoutGroup.Constraint constraint, int constraintCount, TextAnchor childAlignment, Action<GridComponent.GridBuilder> grid)
        {
            GridComponent gridComponent = N<GridComponent>();
            var builder = new GridComponent.GridBuilder(gridComponent);
            builder.GetGrid().constraint = constraint;
            builder.GetGrid().constraintCount = constraintCount;
            builder.GetGrid().childAlignment = childAlignment;
            grid(builder);
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
            Action<GridComponent.GridBuilder> grid)
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
        public static GridComponent Grid(Action<GridComponent.GridBuilder> grid) => Grid(GridLayoutGroup.Constraint.Flexible, 0, grid);
        
        /// <summary>
        /// A grid layout element based on Unity's <see cref="GridLayoutGroup"/>.
        /// </summary>
        /// <param name="childAlignment">The alignment of the grid's children within it.</param>
        /// <param name="grid"><see cref="GridComponent.GridBuilder"/> to configure the grid further.</param>
        /// <returns>
        /// The resulting <see cref="GridComponent"/>.
        /// </returns>
        public static GridComponent Grid(TextAnchor childAlignment, Action<GridComponent.GridBuilder> grid) => Grid(GridLayoutGroup.Constraint.Flexible, 0, childAlignment, grid);


        /// <summary>
        /// An input / text field based on <see cref="TMP_InputField"/>.
        /// </summary>
        /// <param name="placeholder">Placeholder string for the input field.</param>
        /// <param name="contentType"><see cref="TMP_InputField.ContentType"/> for the input field.</param>
        /// <param name="builder">(Optional) <see cref="InputComponent.InputBuilder"/> to configure the input further.</param>
        /// <returns>
        /// The resulting <see cref="InputComponent"/>.
        /// </returns>
        public static InputComponent Input(string placeholder, TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard, [CanBeNull] Action<InputComponent.InputBuilder> builder = null)
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
        public static InputComponent Input(string placeholder, Action<InputComponent.InputBuilder> builder) =>
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
        public static SliderComponent Slider(Range range, Action<SliderComponent.SliderBuilder> builder,
            Action<float> onValueChanged = null)
        {
            SliderComponent slider = N<SliderComponent>();
            slider.SetRange(range);

            var build = new SliderComponent.SliderBuilder(slider);
            build.HandleWidth(SliderComponent.DefaultSize.y);
            builder(build);

            if (onValueChanged != null)
            {
                slider.OnValueChanged.AddListener(newVal => onValueChanged(newVal));
            }

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