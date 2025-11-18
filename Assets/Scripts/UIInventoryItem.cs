using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInventoryItem : MonoBehaviour
{
    private InventoryItem item;

    public Image itemIconImage;
    public TMP_Text itemQuantityText;
  

    public void SetItem(InventoryItem newItem)
    {
        item = newItem;
        // Update UI elements here based on item properties
        itemIconImage.sprite = item.collectable.icon;
        itemQuantityText.text = item.quantity.ToString();
    }

    public InventoryItem GetItem()
    {
        return item;
    }

    public void IncrementQuantity(int amount = 1)
    {
        item.quantity += amount;
        itemQuantityText.text = item.quantity.ToString();
    }
    public void DecrementQuantity(int amount = 1)
    {
        item.quantity -= amount;
        if (item.quantity < 0)
        {
            item.quantity = 0;
        }
        itemQuantityText.text = item.quantity.ToString();
    }
}
