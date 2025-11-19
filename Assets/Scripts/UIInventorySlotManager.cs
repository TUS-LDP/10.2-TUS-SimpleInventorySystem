using System.Collections.Generic;
using UnityEngine;

public class UIInventorySlotManager : MonoBehaviour
{
    public RectTransform inventoryPanel;  // The parent object that holds the inventory UI slots

    // Reference to the InventoryManager
    public InventoryManager inventoryManager;

    public GameObject slotPrefab; // The UI panel for the inventory

    private List<UIInventorySlot> itemsInInventory = new List<UIInventorySlot>();

    private UIInventorySlot activeSlot;

    // Called when the object becomes enabled and active
    void OnEnable()
    {
        inventoryManager.OnItemAdded += HandleItemAdded;
        inventoryManager.OnItemRemoved += HandleItemRemoved;
        inventoryManager.OnActiveItemChanged += HandleActiveItemChanged;
    }

    // Called when the behaviour becomes disabled or inactive
    void OnDisable()
    {
        inventoryManager.OnItemAdded -= HandleItemAdded;
        inventoryManager.OnItemRemoved -= HandleItemRemoved;
        inventoryManager.OnActiveItemChanged -= HandleActiveItemChanged;
    }

    private void HandleItemAdded(InventoryItem item)
    {
        // Update UI to reflect item addition

        // Let's see if the item is already in the itemsInInventory list
        UIInventorySlot itemSlot = itemsInInventory.Find(ui => ui.GetItem().theItemsSO.displayName == item.theItemsSO.displayName);
        if (itemSlot != null) 
        {
            // Item already exists in UI, just increment quantity
            itemSlot.SetQuantity(item.quantity);
        }
        else
        {
            // Item does not exist in UI, create a new UIInventoryItem
            UIInventorySlot newSlot = Instantiate(slotPrefab, inventoryPanel).GetComponent<UIInventorySlot>();
            newSlot.SetItem(item);
            itemsInInventory.Add(newSlot);
        }

        if (itemsInInventory.Count == 1)
        {
            // If this is the first item added, set it as the active slot
            activeSlot = itemsInInventory[0];
            activeSlot.GetComponent<UnityEngine.UI.Image>().color = Color.yellow; // Highlight active slot
        }
    }

    private void HandleItemRemoved(InventoryItem item)
    {
        // Update UI to reflect item removal
        UIInventorySlot itemSlot = itemsInInventory.Find(ui => ui.GetItem().theItemsSO.displayName == item.theItemsSO.displayName);
        
        // Did we find an item slot containing the item we want to remove
        if (itemSlot != null)
        {
            // Have we still 1 of more of the item?
            if (item.quantity > 0)
            {
                // Just update the quantity
                itemSlot.SetQuantity(item.quantity);
            }
            else
            {
                // Okay, lets remove the item slot from the inventory list
                itemsInInventory.Remove(itemSlot);

                // Let's also destroy the UI GameObject
                Destroy(itemSlot.gameObject);
            }
        }
    }

    private void HandleActiveItemChanged(InventoryItem newActiveItem)
    {
        // Find the slot corresponding to the new active item
        UIInventorySlot itemSlot = itemsInInventory.Find(ui => ui.GetItem().theItemsSO.displayName == newActiveItem.theItemsSO.displayName);
        
        SetActiveSlot(itemSlot);
    }

    // I wrote this method because we need to do a few things whenever the active slot changes. We need to "unhighlight" the
    // current active slot, set the new active slot, and then highlight the new active slot.
    private void SetActiveSlot(UIInventorySlot slot)
    {
        // Remove highlight from previous active slot
        if (activeSlot != null)
        {
            activeSlot.GetComponent<UnityEngine.UI.Image>().color = Color.white; 
        }

        // Set the new active slot
        activeSlot = slot;

        // Highlight the new active slot
        if (activeSlot != null)
        {
            activeSlot.GetComponent<UnityEngine.UI.Image>().color = Color.yellow;
        }
    }
}
