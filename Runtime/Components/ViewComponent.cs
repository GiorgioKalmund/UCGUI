using UnityEngine;
using UnityEngine.EventSystems;

namespace UCGUI
{
    /// <summary>
    /// UCGUI's basic ViewComponent.
    /// <br></br>
    /// Acts as an instantiation instance for a basic <see cref="AbstractViewComponent"/>.
    /// </summary>
    public partial class ViewComponent : AbstractViewComponent
    {
        protected ViewComponent() {}

        protected override void Create() { }

        protected override void Initialize()
        {
            DisplayName = "ViewComponent";
        }

        public override void Render() { }
    }
}