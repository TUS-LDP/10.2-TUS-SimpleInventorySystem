using System.Collections.Generic;
using UnityEngine;

public class UIInventoryManager : MonoBehaviour
{
    public RectTransform inventoryPanel;  // The parent object that holds the inventory UI items

    // Reference to the InventoryManager
    public InventoryManager inventoryManager;

    public GameObject inventoryUI; // The UI panel for the inventory

    private List<UIInventoryItem> itemsInInventory = new List<UIInventoryItem>();

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
        Debug.Log($"Item Added: {item.collectable.displayName}, Quantity: {item.quantity}");

        // Let's see if the item is already in the itemsInInventory list
        UIInventoryItem uiItem = itemsInInventory.Find(ui => ui.GetItem().collectable.displayName == item.collectable.displayName);
        if (uiItem != null) 
        {
            // Item already exists in UI, just increment quantity
            uiItem.IncrementQuantity(item.quantity);
        }
        else
        {
            // Item does not exist in UI, create a new UIInventoryItem
            Instantiate(inventoryUI, inventoryPanel).GetComponent<UIInventoryItem>().SetItem(item);
        }
    }

    private void HandleItemRemoved(InventoryItem item)
    {
        // Update UI to reflect item removal
        Debug.Log($"Item Removed: {item.collectable.displayName}, Quantity: {item.quantity}");
    }

    private void HandleActiveItemChanged(InventoryItem activeItem)
    {
        // Update UI to reflect active item change
        Debug.Log($"Active Item Changed: {activeItem.collectable.displayName}");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
