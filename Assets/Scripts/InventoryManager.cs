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
    public void AddItem(CollectableItem itemToAdd, int quantity)
    {
        InventoryItem item = itemsInInventory.Find(i => i.collectable.displayName == itemToAdd.displayName);

        if (item != null)
        {
            item.quantity += quantity;
        }
        else
        {
            item = new InventoryItem();
            item.collectable = itemToAdd;
            item.quantity = quantity;
            itemsInInventory.Add(item);
        }

        if (activeItemIndex == -1)
        {
            activeItemIndex = 0;
        }

        OnItemAdded?.Invoke(item);
    }

    public void RemoveItem(CollectableItem itemToRemove, int quantity)
    {
        InventoryItem existingItem = itemsInInventory.Find(i => i.collectable == itemToRemove);
        if (existingItem != null)
        {
            existingItem.quantity -= quantity;
            if (existingItem.quantity <= 0)
            {
                itemsInInventory.Remove(existingItem);
                if (itemsInInventory.Count == 0)
                {
                    activeItemIndex = -1;
                }
                else if (activeItemIndex >= itemsInInventory.Count)
                {
                    activeItemIndex = itemsInInventory.Count - 1;
                }
            }
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
        }
    }

    public void CycleActiveItem()
    {
        if (itemsInInventory.Count == 0)
        {
            activeItemIndex = -1;
            return;
        }

        activeItemIndex = (activeItemIndex + 1) % itemsInInventory.Count;
    }
}
