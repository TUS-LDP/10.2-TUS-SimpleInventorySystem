using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// This script manages the player's inventory, allowing adding, removing, and cycling through items. I have it attached
// to the player capsule GameObject but it could be attached elsewhere as needed (e.g., a GameManager object).
public class InventoryManager : MonoBehaviour
{
    // List of items in the inventory
    public List<InventoryItem> itemsInInventory = new List<InventoryItem>();

    // Index of the currently active item. I've initialized it to -1 to indicate no active item. I've also made it
    // serialized so you can see it in the Inspector for testing purposes.
    [SerializeField] private int activeItemIndex = -1;

    // Events. All three events pass the relevant InventoryItem as a parameter as it contains all the info needed.
    public event Action<InventoryItem> OnItemAdded;
    public event Action<InventoryItem> OnItemRemoved;
    public event Action<InventoryItem> OnActiveItemChanged;


    // Adds an item to the inventory or increases its quantity if it already exists.
    public void AddItem(CollectableItem itemToAddSO, int amtToAdd = 1)
    {
        InventoryItem item = itemsInInventory.Find(i => i.theItemsSO.displayName == itemToAddSO.displayName);

        if (item != null)
        {
            item.quantity += amtToAdd;
        }
        else
        {
            item = new InventoryItem();
            item.theItemsSO = itemToAddSO;
            item.quantity = amtToAdd;
            itemsInInventory.Add(item);
        }

        OnItemAdded?.Invoke(item);

        if (activeItemIndex == -1)
        {
            SetActiveItem(0);
        }        
    }

    public void RemoveItem(CollectableItem itemToRemove, int quantity)
    {
        InventoryItem item = itemsInInventory.Find(i => i.theItemsSO.displayName == itemToRemove.displayName);

        if (item != null)
        {
            item.quantity -= quantity;
            if (item.quantity <= 0)
            {
                // Remember, we always remove the active item, so after we remove this item we will need to update the active item.
                itemsInInventory.Remove(item);

                // Let's determine the new active item. First check if there are any items left in the inventory.
                if (itemsInInventory.Count > 0)
                {
                    // If we removed the last item in the list, then lets set the active item to the new last item.
                    // Let's assume it starts as:
                    // List [S, G , B] has a Count value of 3. 
                    // If we removed B i.e. index 2, the List would be
                    // [S, G] with a Count value of 2. 
                    // 
                    // So the index of the item we removed (2) is now equal to the new Count (2).
                    // In this case we need to set the new active item to index 1 (the new last item).
                    if (activeItemIndex == itemsInInventory.Count)
                    {
                        // Rather than setting activeItemIndex directly, I call the SetActiveItem() method to ensure 
                        // the OnActiveItemChanged event is invoked.
                        SetActiveItem(itemsInInventory.Count - 1);
                    }
                    else
                    {
                        // We don't need to change the activeItemIndex. If we removed an item in the middle of the list, the next item
                        // will have shifted down to take it's place, so the activeItemIndex is still valid. However, we
                        // do need to invoke the OnActiveItemChanged event to notify any listeners that the active item
                        // has changed (it's still the same index, but the item at that index has changed).
                        SetActiveItem(activeItemIndex);
                    }
                }
                else if (itemsInInventory.Count == 0)
                {
                    activeItemIndex = -1;
                }
            }

            OnItemRemoved?.Invoke(item);
        }
                
    }

    public InventoryItem GetActiveItem()
    {
        if (activeItemIndex >= 0 && activeItemIndex < itemsInInventory.Count)
        {
            return itemsInInventory[activeItemIndex];
        }
        return null;
    }

    public void SetActiveItem(int index)
    {
        if (index >= 0 && index < itemsInInventory.Count)
        {
            activeItemIndex = index;

            OnActiveItemChanged?.Invoke(GetActiveItem());
        }
    }

    public void CycleActiveItem()
    {
        if (itemsInInventory.Count == 0)
        {
            activeItemIndex = -1;
            return;
        }

        int newActiveItemIndex = (activeItemIndex + 1) % itemsInInventory.Count;
        SetActiveItem(newActiveItemIndex);
        
    }
}
