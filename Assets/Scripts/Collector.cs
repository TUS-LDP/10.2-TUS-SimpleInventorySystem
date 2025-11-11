using UnityEngine;

public class Collector : MonoBehaviour
{
    public InventoryManager playerInventory;
    void OnTriggerEnter(Collider other)
    {
        // Below I use the relatively new pattern matching feature to both check the type and assign it to a variable. 
        //          other.GetComponent<CollectableController>() is CollectableController collectable
        //
        // The "is" keyword checks if the other GameObject has a CollectableController component, and if so, assigns it 
        // to the variable 'collectable'.
        // The "old way" would be to use GetComponent and then check if the result is not null. For example:
        //
        // CollectableController collectable = other.GetComponent<CollectableController>();
        // if (collectable != null)
        // {
        //     // Use collectable here
        // }
        //
        // The pattern matching approach is more concise and expressive.
        //
        // So..., if the other collider belongs to a GameObject with the tag "Collectable" and has a CollectableController component,
        // we "enter" the if block.
        if (other.CompareTag("Collectable") && other.GetComponent<CollectableController>() is CollectableController collectable)
        {
            Debug.Log($"Collected: {collectable.collectableItem.displayName} of type {collectable.collectableItem.itemType} with value {collectable.collectableItem.value}");

            if (playerInventory != null)
            {
                playerInventory.AddItem(collectable.collectableItem, 1);
            }
            
            Destroy(other.gameObject); // Remove the collectable from the scene after collection.            
        }
    }
}
