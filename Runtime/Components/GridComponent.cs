using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UCGUI
{
    public class GridComponent : LayoutComponent
    {
        protected GridComponent() {}
        
        public GridLayoutGroup grid;
        public override void Awake()
        {
            base.Awake();

            grid = gameObject.GetOrAddComponent<GridLayoutGroup>();
        }

        public override void Start()
        {
            base.Start();
            this.DisplayName("Grid");
        }

        public class GridBuilder
        {
            private GridComponent _grid;
            public GridBuilder(GridComponent grid) => _grid = grid;

            public void SpacingVertical(float spacing)
            {
                var s = _grid.grid.spacing;
                s.y = spacing;
                _grid.grid.spacing = s;
            }
            public void SpacingHorizontal(float spacing)
            {
                var s = _grid.grid.spacing;
                s.x = spacing;
                _grid.grid.spacing = s;
            }
            public void Spacing(Vector2 spacing) =>  _grid.grid.spacing = spacing;
            public void Spacing(float x, float y) => Spacing(new Vector2(x, y));
            public void CellSize (Vector2 cellSize) => _grid.grid.cellSize = cellSize;
            public void CellSize(float x, float y) => CellSize(new Vector2(x, y));
            public void Padding(RectOffset padding) => _grid.grid.padding = padding;
            public void Padding(PaddingSide side, int amount) => _grid.grid.Padding(side, amount);
            public void PaddingAdd(PaddingSide side, int amount) => _grid.grid.PaddingAdd(side, amount);
            public void Add(params BaseComponent[] element) => _grid.Add(element);
            public GridLayoutGroup GetGrid() => _grid.grid;
        }
    }
}