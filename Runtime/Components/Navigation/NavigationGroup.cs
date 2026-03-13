using System.Collections.Generic;
using System.Linq;
using UGUI;
using UnityEditor;
using UnityEngine;
using Entry = UCGUI.DirectionMap.Entry;

namespace UCGUI
{

    /// <summary>
    /// Calculates the most intuitive spatial navigation pattern.
    /// <remarks>
    /// Based on ideas presented by Hannes Karttunen's Bachelor Thesis:
    /// 'Designing Logic and Tools for Automated Spatial Navigation Mapping for User Interfaces'
    /// https://www.theseus.fi/bitstream/handle/10024/894496/Karttunen_Hannes.pdf;jsessionid=767E643A8B7A3E5DA10D8618539C2E84?sequence=3
    /// 
    /// as well as the Netflix Technical Blog:
    /// 'Pass the Remote: User Input on TV Devices'
    /// https://netflixtechblog.com/pass-the-remote-user-input-on-tv-devices-923f6920c9a8
    /// </remarks>
    /// </summary>
    ///
    public class NavigationGroup
    {
        private Dictionary<Entry, DirectionMap> navigationMapping;
        private Dictionary<IFocusable, DirectionMap> focusMapping;
        private Entry activeEntry;
        private NavigationNode root;
        public float MaxSearchAngle { get; }
        public string GroupId { get; private set; } = Defaults.Focus.DefaultNavigationGroup;

        public NavigationGroup(NavigationNode root, float searchAngle = 22.5f)
        {
            this.root = root;
            MaxSearchAngle = searchAngle;
            navigationMapping = new Dictionary<Entry, DirectionMap>();
            focusMapping = new Dictionary<IFocusable, DirectionMap>();
        }

        public void CalculateConnections()
        {
            if (root == null)
                UCGUILogger.LogWarning("[NavigationGroup]: No root to calculate connections.");

            root?.CalculateConnections(navigationMapping, MaxSearchAngle);
            focusMapping = navigationMapping.ToDictionary(
                p =>
                {
                    p.Key.focus.FocusGroup = GroupId;
                    return p.Key.focus;
                },
                p => p.Value);
        }

        public void Print()
        {
            Debug.Log($"{navigationMapping.Keys.Count} entries.");
            foreach (var (entry, map) in navigationMapping)
            {
                Debug.Log($"{entry.gameObject.name}\n" + map);
            }
        }

        public void Left()
        {
            GetActiveMap()?.left?.focus.Focus();
        }
        
        public void Right()
        {
            GetActiveMap()?.right?.focus.Focus();
        }
        
        public void Up()
        {
            GetActiveMap()?.up?.focus.Focus();
        }
        
        public void Down()
        {
            GetActiveMap()?.down?.focus.Focus();
        }

        /// <summary>
        /// Tries to interact with the currently focused element of the group by
        /// checking if it is an <see cref="IInteractable"/> and invoking <see cref="IInteractable.Interact()"/>.
        /// </summary>
        public void Interact()
        {
            var active = IFocusable.GetFocusedElement(GroupId) as IInteractable;
            active?.Interact();
        }

        private void Set(Entry of, Direction dir, Entry to)
        {
            navigationMapping.TryGetValue(of, out var map);
            map.Assign(to, dir);
            focusMapping[of.focus] = map;
            navigationMapping[of] = map;
        }

        private DirectionMap? GetActiveMap()
        {
            var active = IFocusable.GetFocusedElement(GroupId);
            if (active == null)
                return null;
            focusMapping.TryGetValue(active, out var map);
            return map;
        }
        
    
#if UNITY_EDITOR
        public void Draw()
        {
            root?.Draw();
            foreach (var (entry, map) in navigationMapping)
            {
                foreach (var directionVector in DirectionHelper.DirectionVectors)
                {
                    var target = map.Get(directionVector);
                    float scalingFactor = 2f;

                    if (target != null)
                    {
                        Vector3 offset;
                        switch (directionVector)
                        {
                            case Direction.Left:
                            {
                                Gizmos.color = Color.yellow;
                                break;
                            }
                            case Direction.Right:
                            {
                                Gizmos.color = Color.green;
                                break;
                            }
                            case Direction.Up:
                            {
                                Gizmos.color = Color.red;
                                break;
                            }
                            case Direction.Down:
                            {
                                Gizmos.color = Color.blue;
                                break;
                            }
                        }
                        offset = directionVector.GetClockwise().GetVector() * scalingFactor;
                        var start = entry.gameObject.transform.position + offset;
                        var end = target.gameObject.transform.position + offset;
                        Handles.Label((start + end) / 2, $"{directionVector.Name()}", Defaults.Debug.DebugStyle(Gizmos.color, 12));
                        Gizmos.DrawLine(start, end);
                    }
                }
            }
        }
#endif
    }
}