using System;
using System.Collections.Generic;
using System.Linq;
using UGUI;
using UnityEditor;
using UnityEngine;
using Entry = UCGUI.DirectionMap.Entry;

namespace UCGUI
{
    public class NavigationNode
    {
        private NavigationNode parent;
        private readonly List<NavigationNode> children;
        private readonly Dictionary<NavigationNode, Vector2[]> childrenHitPoints;
        private readonly List<Entry> targets;
        private readonly Dictionary<Entry, Vector2[]> targetHitPoints;

        private Vector2? sizeDelta;
        private Vector2? position;
        
        const int verticalSubdivisions = 1;
        const int horizontalSubdivisions = 1;

        public string name;

        private DirectionMap? entryMap;

        private const double OneHundredEightyDividedByPi = 180 / Math.PI;
        
        
        #region Constructor

        public NavigationNode(string n = "NavigationNode")
        {
            name = n;
            children = new List<NavigationNode>();
            childrenHitPoints = new Dictionary<NavigationNode, Vector2[]>();
        }
        
        public NavigationNode(params NavigationNode[] children)
        {
            this.children = children.ToList();
            childrenHitPoints = new Dictionary<NavigationNode, Vector2[]>();
        }
        
        public NavigationNode(string n, params IFocusable[] targets)
        {
            name = n;
            this.targets = targets.Select(t => new Entry(t)).ToList();
            targetHitPoints = new Dictionary<Entry, Vector2[]>();
            foreach (var entry in this.targets)
            {
                List<Vector2> positions = new List<Vector2>(){ entry.gameObject.transform.position };
                CalculateCrossHitpoints(entry.gameObject.transform.position, entry.gameObject.GetRect().sizeDelta, horizontalSubdivisions, verticalSubdivisions,  positions);
                targetHitPoints.Add(entry, positions.ToArray());
            }
            entryMap = new DirectionMap();
        }

        private void CalculateCrossHitpoints(Vector2 center, Vector2 dimensions, int hSubdivisions, int vSubdivisions, List<Vector2> elements)
        {
            float halfWidth = dimensions.x * 0.5f;
            float halfHeight = dimensions.y * 0.5f;
            Vector2 left = center - new Vector2(halfWidth, 0);
            Vector2 right = center + new Vector2(halfWidth, 0);
            Vector2 up = center + new Vector2(0, halfHeight);
            Vector2 down = center - new Vector2(0, halfHeight);
            elements.AddRange(new []{left, right, up, down});
            float intervalLengthH = halfWidth / (hSubdivisions + 1);
            float intervalLengthV = halfHeight / (vSubdivisions + 1);
            for (int i = 0; i < hSubdivisions ; i++)
            {
                float hLen = intervalLengthH * (i + 1);
                elements.Add(left + new Vector2(hLen, 0));
                elements.Add(right - new Vector2(hLen, 0));
            }
            
            for (int i = 0; i < vSubdivisions ; i++)
            {
                float vLen = intervalLengthV * (i + 1);
                elements.Add(up - new Vector2(0, vLen));
                elements.Add(down + new Vector2(0, vLen));
            }
        }
        
        public NavigationNode(params Entry[] targets)
        {
            this.targets = targets.ToList();
        }

        #endregion

        internal void CalculateConnections(Dictionary<Entry, DirectionMap> inMap, float maxSearchAngle)
        {
            if (IsLeaf())
            {
                foreach (var source in targets)
                {
                    inMap.TryGetValue(source, out var map);
                    foreach (var directionVector in DirectionHelper.DirectionVectors)
                    {
                        double maxVal = -1;
                        Entry max = null;
                       
                        FindClosestEntry(source, directionVector, maxSearchAngle, ref maxVal, ref max);

                        
                        map.Assign(max, directionVector);
                    }

                    inMap[source] = map;
                }
            }
            else
            {
                foreach (var navigationNode in children)
                {
                    navigationNode.CalculateConnections(inMap, maxSearchAngle);
                }
            }
        }

        private void FindClosestEntry(Entry source, Direction directionVector, float maxSearchAngle, ref double maxVal, ref Entry max)
        {
            foreach (var target in targets)
            {
                if (target == source)
                    continue;

                foreach (Vector3 targetPos in targetHitPoints[target])
                {
                    Vector2 dir = targetPos - source.gameObject.transform.position;
                    Vector2 dirN = dir;
                    dirN.Normalize();

                    double dot = Vector2.Dot(directionVector.GetVector(), dirN);

                    if (dot < 0)
                        continue;

                    double alpha = Math.Acos(dot) * OneHundredEightyDividedByPi;

                    if (alpha > maxSearchAngle)
                        continue;

                    double heuristic = dot / dir.sqrMagnitude * 100;
                    if (heuristic > maxVal)
                    {
                        max = target;
                        maxVal = heuristic;
                    }
                }
            }

            if (max == null && !IsRoot())
            {
                //Debug.Log($"{source.gameObject.name} doesnt have an entry for: {directionVector}. Attempting to find appropriate element in parents" );
                FindClosestNode(directionVector, maxSearchAngle, out var closest);
                //Debug.Log($"Found the closest container in that direction: {closest?.name}");
                if (closest != null)
                {
                    max = closest.GetDirectionalEntryOrDefault(directionVector.GetOpposite());
                    //Debug.Log($"It's entry point is: {max?.gameObject.name}");
                }
            }
        }

        private void FindClosestNode(Direction directionVector, float maxSearchAngle, out NavigationNode closest)
        {
            if (IsRoot())
            {
                //Debug.Log($"{name} is root");
                closest = null;
                return;
            }
            
            NavigationNode max = null;
            double maxVal = -1;
            //Debug.Log($"{name} is searching for viable sibling containers {parent.children.Count- 1}");
            foreach (var navigationNode in parent.children)
            {
                if (navigationNode == this)
                    continue;
                
                Vector2 dir = navigationNode.GetPosition() - GetPosition();
                Vector2 dirN = dir;
                dirN.Normalize();

                double dot = Vector2.Dot(directionVector.GetVector(), dirN);

                if (dot < 0)
                {
                    //Debug.Log($"pruned {navigationNode.name}. wrong dot {dot}");
                    continue;
                }

                double alpha = Math.Acos(dot) * (180 / Math.PI);

                if (alpha > maxSearchAngle)
                {
                    //Debug.Log($"pruned {navigationNode.name}. wrong angle {alpha}");
                    continue;
                }

                double heuristic = dot / dir.magnitude * 100;
                if (heuristic > maxVal)
                {
                    max = navigationNode;
                    maxVal = heuristic;
                }
            }

            if (max == null && !IsRoot())
                parent.FindClosestNode(directionVector, maxSearchAngle, out max);
            closest = max;
        }

        
        public void Manual(Vector2 pos, Vector2 size)
        {
            position = pos;
            sizeDelta = size;
        }

        private void _AddChild(NavigationNode node)
        {
            if (IsLeaf())
                throw new InvalidOperationException("[NavigationNode]: Can't add child to leaf node.");
            
            children.Add(node);
            node.parent = this;
            Invalidate();
        }

        public void AddChild(params NavigationNode[] nodes)
        {
            foreach (var navigationNode in nodes)
                _AddChild(navigationNode);
        }
        
        public void AddTarget(Entry node)
        {
            if (!IsLeaf())
                throw new InvalidOperationException("[NavigationNode]: Can't add target to non-leaf node.");
            
            targets.Add(node);
            Invalidate();
        }
        
        public Vector2 GetPosition()
        {
            if (position.HasValue)
                return position.Value;
            
            CalculateBounds(out var min, out var max);
            CalculateNodeHitPoints();
            position = (min + max) * 0.5f;
            sizeDelta = max - min;
            return position.Value;
        }

        public Vector2 GetSizes()
        {
            if (!sizeDelta.HasValue)
                GetPosition();

            return sizeDelta ?? throw new InvalidOperationException("[NavigationNode]: No sizeDelta could be determined!");
        }

        private void CalculateBounds(out Vector2 min, out Vector2 max)
        {
            float xMin = float.PositiveInfinity;
            float yMin = float.PositiveInfinity;
            float xMax = float.NegativeInfinity;
            float yMax = float.NegativeInfinity;

            if ((IsLeaf() && targets.Count == 0) || (!IsLeaf() && children.Count == 0))
            {
                UCGUILogger.LogWarning($"Node {name} is empty. Consider removing it.");
                min = new Vector2(-50, 50);
                max = new Vector2(50, 50);
                return;
            }

            if (IsLeaf())
            {
                foreach (var target in targets)
                {
                    var rect = target.gameObject.GetRect();
                    Vector3[] corners = new Vector3[4];
                    rect.GetWorldCorners(corners);

                    foreach (var c in corners)
                    {
                        if (c.x < xMin) xMin = c.x;
                        if (c.y < yMin) yMin = c.y;
                        if (c.x > xMax) xMax = c.x;
                        if (c.y > yMax) yMax = c.y;
                    }
                }
            }
            else
            {
                foreach (var child in children)
                {
                    var childMin = child.Min;
                    var childMax = child.Max;

                    if (childMin.x < xMin) xMin = childMin.x;
                    if (childMin.y < yMin) yMin = childMin.y;
                    if (childMax.x > xMax) xMax = childMax.x;
                    if (childMax.y > yMax) yMax = childMax.y;
                }
            }

            min = new Vector2(xMin, yMin);
            max = new Vector2(xMax, yMax);
        }

        private void CalculateNodeHitPoints()
        {
            if (IsLeaf())
                return;
            
            foreach (var navigationNode in children)
            {
                List<Vector2> positions = new List<Vector2>(){ navigationNode.GetPosition() };
                CalculateCrossHitpoints(navigationNode.GetPosition(), navigationNode.GetSizes(), horizontalSubdivisions, verticalSubdivisions,  positions);
                childrenHitPoints.Add(navigationNode, positions.ToArray());
            }
        }

        private Vector2 Min => GetPosition() - GetSizes() * 0.5f;
        private Vector2 Max => GetPosition() + GetSizes() * 0.5f;

        public bool IsLeaf()
        {
            return targets != null;
        }
        public bool IsRoot()
        {
            return parent == null;
        }
        
        private void Invalidate()
        {
            position = null;
            sizeDelta = null;
            parent?.Invalidate();
        }

        private Entry GetEntryFirst()
        {
            return targets?.FirstOrDefault() ?? children.FirstOrDefault()?.GetEntryFirst();
        }

        private Entry GetDirectionalEntryOrDefault(Direction forDirection)
        {
            return entryMap?.Get(forDirection) ?? GetEntryFirst();
        }

        /// <summary>
        /// WARNING: Expensive as there is currently no reverse lookup. A tree BFS search has to be performed for every call :(.
        /// </summary>
        public void SetEntry(Direction direction, IFocusable to)
        {
            entryMap ??= new DirectionMap();
            var match = FindEntryFromFocusableInTree(to);
            if (match == null)
                UCGUILogger.LogWarning($"[NavigationNode.SetEntry]: No matching entry found for {to}!");
            var map = entryMap.Value;
            map.Assign(match, direction);
            entryMap = map;
        }

        private Entry FindEntryFromFocusableInTree(IFocusable forFocusable)
        {
            var match = targets?.Find(t => t.focus == forFocusable);
            if (match == null && children != null)
            {
                foreach (var navigationNode in children)
                {
                    match = navigationNode.FindEntryFromFocusableInTree(forFocusable);
                    if (match != null)
                        break;
                }
            }

            return match;

        }

        #if UNITY_EDITOR
        public void Draw()
        {
            Gizmos.DrawWireCube(GetPosition(), GetSizes());
            Handles.Label(GetPosition(),  $"{name}", Defaults.Debug.DebugRed(8));

            if (IsLeaf())
            {
                DrawTargetHitPoints();
            }
            else
            {
                DrawChildrenHitPoints();
            }
        }

        private void DrawTargetHitPoints()
        {
            foreach (var (target, points) in targetHitPoints)
            {
                foreach (var vector2 in points)
                {
                    Gizmos.color = Color.deepPink;
                    Gizmos.DrawSphere(vector2, 1);
                    Gizmos.color = Color.red;
                }
            }
        }

        private void DrawChildrenHitPoints()
        {
            foreach (var (child, points) in childrenHitPoints)
            {
                foreach (var vector2 in points)
                {
                    Gizmos.color = Color.aquamarine;
                    Gizmos.DrawSphere(vector2, 1);
                    Gizmos.color = Color.red;
                }
                    
                child.Draw();
            }

        }
        #endif
    }
}