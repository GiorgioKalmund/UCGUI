namespace UCGUI
{
    /// <summary>
    /// UCGUI's basic ViewComponent.
    /// <br></br>
    /// Acts as an instantiation instance for a basic <see cref="AbstractViewComponent"/>.
    /// </summary>
    public class ViewComponent : AbstractViewComponent
    {
        protected ViewComponent() {}

        protected override void Create()
        {
            DisplayName = "ViewComponent";
        }

        protected override void Initialize() { }

        public override void Render() { }
    }
}