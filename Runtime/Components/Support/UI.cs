using System.Collections;
using UnityEngine;

namespace UCGUI
{

    public static partial class UI
    {
        public static string DefaultName => "Component";

        // Object creation
        public static GameObject CreateEmptyGameObjectWithParent(Transform parent, bool worldPositionStays,
            string name = "")
        {
            GameObject toReturn = new GameObject(name);
            toReturn.AddComponent<RectTransform>();
            if (parent)
                toReturn.transform.SetParent(parent, worldPositionStays);
            return toReturn;
        }

        public static T N<T>(BaseComponent parent, string name = "", bool worldPositionStays = false)
            where T : BaseComponent
        {
            return N<T>(name, parent.gameObject.transform, worldPositionStays);
        }

        public static T N<T>(Transform parent, string name = "", bool worldPositionStays = false)
            where T : BaseComponent
        {
            return N<T>(name, parent, worldPositionStays);
        }

        public static T N<T>(string name = "", Transform parent = null, bool worldPositionStays = false)
            where T : BaseComponent
        {
            GameObject toReturn = CreateEmptyGameObjectWithParent(parent, worldPositionStays);
            var t = toReturn.AddComponent<T>();
            t.DisplayName(name);
            return t;
        }

        // Casting between types
        public static V Cast<V>(this BaseComponent renderable) where V : BaseComponent
        {
            return (V)(object)renderable;
        }

        // Naming
        public static T DisplayName<T>(this T renderable, string displayName) where T : BaseComponent
        {
            renderable.DisplayName = displayName;
            return renderable;
        }

        public static T SafeDisplayName<T>(this T renderable, string displayName) where T : BaseComponent
        {
            if (string.IsNullOrEmpty(renderable.DisplayName) || displayName.Equals(DefaultName))
                renderable.DisplayName(displayName);
            return renderable;
        }

        // Parent
        public static T Parent<T>(this T renderable, Transform parent, bool worldPositionStays = false)
            where T : BaseComponent
        {
            renderable.GetRect().SetParent(parent, worldPositionStays);
            return renderable;
        }

        public static T Parent<T>(this T renderable, Behaviour parent, bool worldPositionStays = false)
            where T : BaseComponent
        {
            return Parent(renderable, parent.GetTransform(), worldPositionStays);
        }

        // Positioning
        public static T Pos<T>(this T renderable, Vector3 anchoredPosition, Space space) where T : BaseComponent
        {
            if (space == Space.World)
                renderable.GetRect().anchoredPosition3D = anchoredPosition;
            else
                renderable.Pos(renderable.transform.TransformDirection(anchoredPosition));
            return renderable;
        }

        public static T Pos<T>(this T renderable, Vector2 anchoredPosition, bool force = false) where T : BaseComponent
        {
            if (renderable.paddingApplied && !force)
            {
                Debug.LogError("Padding has already been applied to " + renderable.DisplayName +
                               ". Cannot apply additional Pos!");
                return renderable;
            }

            Pos(renderable, anchoredPosition, Space.World);
            renderable.posApplied = !force;
            return renderable;
        }

        public static T Pos<T>(this T renderable, float x, float y) where T : BaseComponent
        {
            return Pos(renderable, new Vector2(x, y));
        }

        public static Vector2 GetPos<T>(this T renderable) where T : BaseComponent
        {
            return renderable.GetRect().anchoredPosition;
        }

        public static Vector3 GetPos3D<T>(this T renderable) where T : BaseComponent
        {
            return renderable.GetRect().anchoredPosition3D;
        }

        public static T Offset<T>(this T renderable, Vector3 offset) where T : BaseComponent
        {
            return Pos(renderable, renderable.GetPos3D() + offset);
        }

        public static T Offset<T>(this T renderable, Vector2 offset) where T : BaseComponent
        {
            return Pos(renderable, renderable.GetPos() + offset);
        }

        public static T Offset<T>(this T renderable, float xOffset, float yOffset, float zOffset = 0)
            where T : BaseComponent
        {
            return Offset(renderable, new Vector3(xOffset, yOffset, zOffset));
        }

        public static T OffsetMin<T>(this T renderable, Vector2 offsetMin) where T : BaseComponent
        {
            renderable.GetRect().offsetMin = offsetMin;
            return renderable;
        }

        public static T OffsetMin<T>(this T renderable, float x, float y) where T : BaseComponent
        {
            return OffsetMin(renderable, new Vector2(x, y));
        }

        public static T OffsetMax<T>(this T renderable, Vector2 offsetMax) where T : BaseComponent
        {
            renderable.GetRect().offsetMin = offsetMax;
            return renderable;
        }

        public static T OffsetMax<T>(this T renderable, float x, float y) where T : BaseComponent
        {
            return OffsetMax(renderable, new Vector2(x, y));
        }

        public static T Stretch<T>(this T renderable) where T : BaseComponent
        {
            var rt = renderable.GetRect();
            var om = rt.offsetMin;
            om.x = 0f;
            rt.offsetMin = om;
            var oM = rt.offsetMax;
            oM.x = 0f;
            rt.offsetMax = oM;
            return renderable;
        }

        public static T RectOffsets<T>(this T renderable, RectOffset offset) where T : BaseComponent
        {
            var rt = renderable.GetRect();
            rt.offsetMin = new Vector2(offset.left, offset.bottom);
            rt.offsetMax = new Vector2(-offset.right, -offset.top);
            return renderable;
        }

        // Rotation
        public static T Rotation<T>(this T renderable, Quaternion rotation) where T : BaseComponent
        {
            renderable.GetRect().rotation = rotation;
            return renderable;
        }

        public static T Rotation<T>(this T renderable, float zRotation) where T : BaseComponent
        {
            return Rotation(renderable, Quaternion.Euler(0, 0, zRotation));
        }

        public static T LocalRotation<T>(this T renderable, Quaternion rotation) where T : BaseComponent
        {
            renderable.GetRect().localRotation = rotation;
            return renderable;
        }

        public static T LocalRotation<T>(this T renderable, float zRotation) where T : BaseComponent
        {
            return LocalRotation(renderable, Quaternion.Euler(0, 0, zRotation));
        }

        public static T RotateRel<T>(this T renderable, Vector3 rotation, Space space) where T : BaseComponent
        {
            renderable.GetRect().Rotate(rotation, space);
            return renderable;
        }

        public static T RotateRel<T>(this T renderable, Quaternion rotation, Space space) where T : BaseComponent
        {
            return RotateRel(renderable, rotation.eulerAngles, space);
        }

        public static T RotateRel<T>(this T renderable, float zRotation, Space space) where T : BaseComponent
        {
            return RotateRel(renderable, new Vector3(0, 0, zRotation), space);
        }

        // Padding
        public static T Padding<T>(this T renderable, PaddingSide side, float padding) where T : BaseComponent
        {
            if (renderable.posApplied)
            {
                Debug.LogError("Pos has already been applied to " + renderable.DisplayName +
                               ". Cannot apply additional Padding!");
                return renderable;
            }

            Vector2 paddingVector = side switch
            {
                PaddingSide.Leading or PaddingSide.Horizontal or PaddingSide.Bottom => new Vector2(padding,
                    renderable.GetPos().y),
                PaddingSide.Trailing or PaddingSide.Top or PaddingSide.Vertical => new Vector2(renderable.GetPos().x,
                    -padding),
                _ => Vector2.zero
            };

            if (side.HasFlag(PaddingSide.Leading))
                renderable.AddWidth(-padding);
            if (side.HasFlag(PaddingSide.Trailing))
                renderable.AddWidth(-padding);
            if (side.HasFlag(PaddingSide.Top))
                renderable.AddHeight(-padding);
            if (side.HasFlag(PaddingSide.Bottom))
                renderable.AddHeight(-padding);

            renderable.paddingApplied = true;
            return Pos(renderable, paddingVector, force:true);
        }

        // Sizing
        // Get Size
        public static float GetWidth<T>(this T renderable) where T : BaseComponent
        {
            return renderable.GetRect().sizeDelta.x;
        }

        public static float GetHeight<T>(this T renderable) where T : BaseComponent
        {
            return renderable.GetRect().sizeDelta.y;
        }

        // Set Size 
        public static T Size<T>(this T renderable, Vector2 sizeDelta) where T : BaseComponent
        {
            renderable.GetRect().sizeDelta = sizeDelta;
            renderable.HandleSizeChanged(sizeDelta.x, sizeDelta.y);
            return renderable;
        }

        public static T Size<T>(this T renderable, float width, float height) where T : BaseComponent
        {
            return Size(renderable, new Vector2(width, height));
        }

        public static T SizeAdd<T>(this T renderable, Vector2 sizeDelta) where T : BaseComponent
        {
            var newSize = renderable.GetRect().sizeDelta + sizeDelta;
            return Size(renderable, newSize);
        }

        public static T Width<T>(this T renderable, float width) where T : BaseComponent
        {
            return Size(renderable, width, renderable.GetHeight());
        }

        public static T Height<T>(this T renderable, float height) where T : BaseComponent
        {
            return Size(renderable, renderable.GetWidth(), height);
        }

        public static Vector2 GetSize<T>(this T renderable) where T : BaseComponent
        {
            return renderable.GetRect().sizeDelta;
        }

        // Add Size
        public static T AddHeight<T>(this T renderable, float extraHeight) where T : BaseComponent
        {
            return Size(renderable, renderable.GetWidth(), renderable.GetHeight() + extraHeight);
        }

        public static T AddWidth<T>(this T renderable, float extraWidth) where T : BaseComponent
        {
            return Size(renderable, renderable.GetWidth() + extraWidth, renderable.GetHeight());
        }

        public static T FullScreen<T>(this T renderable, Canvas canvas) where T : BaseComponent
        {
            if (canvas)
            {
                RectTransform canvasRect = canvas.GetComponent<RectTransform>();
                return Size(renderable, canvasRect.sizeDelta);
            }

            Debug.LogError(renderable.DisplayName +
                           ": Cannot go fullscreen, as no canvas is linked to the object. Please supply a valid canvas to the Fullscreen() function!");
            return renderable;
        }

        // Scaling
        public static T Scale<T>(this T renderable, float widthScaleFactor, float heightScaleFactor)
            where T : BaseComponent
        {
            return Size(renderable, renderable.GetWidth() * widthScaleFactor,
                renderable.GetHeight() * heightScaleFactor);
        }

        public static T LocalScale<T>(this T renderable, Vector3 localScale) where T : BaseComponent
        {
            renderable.GetRect().localScale = localScale;
            return renderable;
        }

        public static T LocalScale<T>(this T renderable, float widthScaleFactor, float heightScaleFactor)
            where T : BaseComponent
        {
            return LocalScale(renderable, new Vector3(widthScaleFactor, heightScaleFactor, 1));
        }

        // Pivots
        public static T Pivot<T>(this T renderable, Vector2 pivot) where T : BaseComponent
        {
            renderable.GetRect().pivot = pivot;
            return renderable;
        }

        public static Vector2 GetVectorFromPivotPos(PivotPosition pivot)
        {
            Vector2 pVector = pivot switch
            {
                PivotPosition.UpperLeft => new Vector2(0, 1f),
                PivotPosition.UpperCenter => new Vector2(0.5f, 1f),
                PivotPosition.UpperRight => new Vector2(1f, 1f),

                PivotPosition.MiddleLeft => new Vector2(0, 0.5f),
                PivotPosition.MiddleCenter => new Vector2(0.5f, 0.5f),
                PivotPosition.MiddleRight => new Vector2(1f, 0.5f),


                PivotPosition.LowerLeft => new Vector2(0, 0f),
                PivotPosition.LowerCenter => new Vector2(0.5f, 0f),
                PivotPosition.LowerRight => new Vector2(1f, 0f),

                _ => new Vector2(-1, -1)
            };
            return pVector;
        }

        public static T Pivot<T>(this T renderable, PivotPosition pivot, bool alsoMoveAnchor = false)
            where T : BaseComponent
        {
            if (alsoMoveAnchor)
                renderable.AnchoredTo(pivot);
            return renderable.Pivot(GetVectorFromPivotPos(pivot));
        }

        // Anchors 
        public static T AnchorMin<T>(this T renderable, Vector2 anchorMin) where T : BaseComponent
        {
            renderable.GetRect().anchorMin = anchorMin;
            return renderable;
        }

        public static T AnchorMin<T>(this T renderable, float x, float y) where T : BaseComponent
        {
            return AnchorMin(renderable, new Vector2(x, y));
        }

        public static T AnchorMax<T>(this T renderable, Vector2 anchorMax) where T : BaseComponent
        {
            renderable.GetRect().anchorMax = anchorMax;
            return renderable;
        }

        public static T AnchorMax<T>(this T renderable, float x, float y) where T : BaseComponent
        {
            return AnchorMax(renderable, new Vector2(x, y));
        }

        public static T AnchoredTo<T>(this T renderable, PivotPosition anchorPosition) where T : BaseComponent
        {
            (Vector2 anchorMinVector, Vector2 anchorMaxVector) = anchorPosition switch
            {
                // Add more variants (like stretching along a side, etc.) here
                _ => (GetVectorFromPivotPos(anchorPosition), GetVectorFromPivotPos(anchorPosition))
            };
            return renderable.AnchorMin(anchorMinVector).AnchorMax(anchorMaxVector);
        }

        // Hierarchy
        public static T NthSibling<T>(this T renderable, int n) where T : BaseComponent
        {
            renderable.GetRect().SetSiblingIndex(n);
            return renderable;
        }

        public static T BringToFront<T>(this T renderable) where T : BaseComponent
        {
            renderable.GetRect().SetAsLastSibling();
            return renderable;
        }

        public static T BringToBack<T>(this T renderable) where T : BaseComponent
        {
            renderable.GetRect().SetAsFirstSibling();
            return renderable;
        }

        // Gameobject 
        public static T SetActive<T>(this T renderable, bool active = true) where T : BaseComponent
        {
            renderable.gameObject.SetActive(active);
            return renderable;
        }

        public static BaseComponent Refresh<V>(this BaseComponent renderable) where V : Behaviour
        {
            V behaviour = renderable.GetComponent<V>();
            if (behaviour)
            {
                renderable.StartCoroutine(RefreshComponent(behaviour));
            }

            return renderable;
        }

        private static IEnumerator RefreshComponent(Behaviour behaviour)
        {
            behaviour.enabled = false;
            yield return new WaitForEndOfFrame();
            behaviour.enabled = true;
        }

        public static Transform GetParent<T>(this T renderable) where T : BaseComponent
        {
            return renderable.GetRect().transform.parent;
        }
    }
}