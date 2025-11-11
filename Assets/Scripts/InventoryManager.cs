using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<InventoryItem> itemsInInventory = new List<InventoryItem>();
    
    public void AddItem(CollectableItem itemToAdd, int quantity)
    {
        InventoryItem existingItem = itemsInInventory.Find(i => i.collectable == itemToAdd);
        if (existingItem != null)
        {
            existingItem.quantity += quantity;
        }
        else
        {
            InventoryItem newItem = new InventoryItem();
            newItem.id = itemsInInventory.Count + 1; // Simple ID assignment
            newItem.collectable = itemToAdd;
            newItem.quantity = quantity;
            itemsInInventory.Add(newItem);
        }
    }
}
