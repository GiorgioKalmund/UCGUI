using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UCGUI.Game
{
    public class Hotbar : ButtonComponent
    {
        public List<HotbarSlot> slots = new List<HotbarSlot>();
        private int _selectedSlotIndex = 0;

        public int SelectedSlotIndex
        {
            get => _selectedSlotIndex;
            set
            {
                if (value < 0)
                    value = slots.Count - 1;
                _selectedSlotIndex = value % slots.Count;
                slots[_selectedSlotIndex].Focus();
            }
        }

        // -- Input Handling -- //
        private InputAction _scrollAction;
        
        /// <summary>
        /// Whether the scroll action is currently enabled if it was set using <see cref="ScrollAction"/>.
        /// </summary>
        public bool Frozen => !_scrollAction?.enabled ?? true;

        public override void Awake()
        {
            base.Awake();
            FitToContents(25).DisabledColor(UnityEngine.Color.white).Lock();
            GetTextComponent().SetActive(false);
        }

        public override void Start()
        {
            base.Start();

            if (_scrollAction != null)
            {
                _scrollAction.performed += context =>
                {
                    Vector2 scrollDelta = context.ReadValue<Vector2>();

                    if (scrollDelta.y > 0f || scrollDelta.x < 0f)
                        ScrollUp();
                    else if (scrollDelta.y < 0f || scrollDelta.x > 0f)
                        ScrollDown();

                };

            }

            slots[0].Focus();
        }

        /// <summary>
        /// Increases the selected slot index by 1.
        /// </summary>
        public void ScrollUp()
        {
            SelectedSlotIndex++;
        }

        /// <summary>
        /// Decreases the selected slot index by 1.
        /// </summary>
        public void ScrollDown()
        {
            SelectedSlotIndex--;
        }

        /// <summary>
        /// Sets the selected slot index.
        /// </summary>
        public void SelectSlot(int index)
        {
            SelectedSlotIndex = index;
        }

        /// <summary>
        /// Disables the scroll action if it was set using <see cref="ScrollAction"/> .
        /// </summary>
        public void Freeze()
        {
            _scrollAction?.Disable();
        }

        /// <summary>
        /// Enables the scroll action if it was set using <see cref="ScrollAction"/> .
        /// </summary>
        public void UnFreeze()
        {
            _scrollAction?.Enable();
        }

        /// <summary>
        /// Toggles freezing and unfreezing of the scroll action if it was set using <see cref="ScrollAction"/>.
        /// </summary>
        public void ToggleFreeze()
        {
            if (Frozen)
                UnFreeze();
            else
                Freeze();
        }

        /// <summary>
        /// Adds new slot object to the hotbar.
        /// </summary>
        /// <param name="slots">The slot template / component to add.</param>
        /// <returns></returns>
        public Hotbar AddSlots(params HotbarSlot[] slots)
        {
            foreach (var hotbarSlot in slots)
            {
                if (!hotbarSlot)
                    return this;

                this.slots.Add(hotbarSlot);
                hotbarSlot.Parent(this).SetActive().SetFocusGroup(FocusGroup);
            }

            return this;
        }

		
        /// <summary>
        /// Removed slot object from the hotbar.
        /// </summary>
        /// <param name="index">The slot index to remove.</param>
        /// <returns></returns>
        public Hotbar RemoveSlot(int index)
        {
            if (index >= 0 && index < slots.Count)
            {
                var toRemove = slots[index];
                slots.RemoveAt(index);
                Destroy(toRemove.gameObject);
            }
            else
            {
                Debug.LogWarning("Trying to access invalid hotbar index: " + index);
            }

            return this;
        }

        /// <summary>
        /// Links an appropriate input action to the hotbar to allow switching of slots using scrolling.
        /// If you want more specific control over how slot switching should work see <see cref="ScrollUp"/>, <see cref="ScrollDown"/> and <see cref="SelectSlot"/> / <see cref="SelectedSlotIndex"/>.
        /// </summary>
        /// <param name="action">The input action, Vector2, to use to scroll between slots.</param>
        public void ScrollAction(InputAction action)
        {
            _scrollAction = action;
        }

    }
}