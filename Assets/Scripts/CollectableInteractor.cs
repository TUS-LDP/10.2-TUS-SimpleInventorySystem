using UnityEngine;
using UnityEngine.InputSystem;

// This script handles interactions with collectable items, allowing the player to pick them up and drop them. It is attached
// to the player character.
public class CollectableInteractor : MonoBehaviour
{
    // Reference to the player's InventoryManager component
    public InventoryManager playerInventory;

    // When the player is in the trigger zone of a collectable item, this variable holds a reference to that item. I use this
    // to know which item to pick up when the player presses the pick-up input.
    private CollectableController collectableAvailableForPickup;

    // Called when the player presses the input to cycle to the next item in their inventory. I have this mapped to the
    // "Tab" key in the Input Actions. The Player Input component on the player capsule is set to use "Broadcast Messages" so
    // Unity will automatically call this method when the input is triggered.
    public void OnCycleItem(InputValue value)
    {
        playerInventory.CycleActiveItem();
    }

    // Called when the player presses the input to drop the currently active item in their inventory. I have this mapped to the
    // "P" key in the Input Actions. 
    public void OnDropItem(InputValue value)
    {
        DropActiveItem();
    }

    public void OnPickUpItem(InputValue value)
    {
        // Have we got a collectable item available to pick up? This gets set when we enter the trigger zone of a collectable item.
        if (collectableAvailableForPickup != null)
        {           
            // The collectableAvailableForPickup variable holds a reference to the CollectableController component which in turn
            // has a property called itemData which is a reference to the CollectableItem ScriptableObject representing the item.
            // I add the scriptable object to the player's inventory with a quantity of 1.
            playerInventory.AddItem(collectableAvailableForPickup.theItemsSO, 1);

            // After picking up the item, I destroy the collectable GameObject from the scene.
            Destroy(collectableAvailableForPickup.gameObject);

            // Reset the reference as we've picked up the item
            collectableAvailableForPickup = null; 
        }
    }

    // When the player enters a trigger collider, this method is called. 
    void OnTriggerEnter(Collider other)
    {
        // Below I use the relatively new "pattern matching" feature in csharp to both check the type and assign it to a variable. 
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
            // Okay, let's store a reference to the collectable item we are in the trigger zone of so we can pick it up later IF
            // the player presses the pick-up input.
            collectableAvailableForPickup = collectable;     
        }
    }

    // Drops an item from the player's inventory into the game world. This method is passed a CollectableItem ScriptableObject
    // which represents the item to drop. 
    // I remove one quantity of the item from the inventory and instantiate its prefab in the game world at the player's position.
    public void DropItem(CollectableItem itemToDrop)
    {
        if (playerInventory != null)
        {
            playerInventory.RemoveItem(itemToDrop, 1);

            // Using the prefab reference from the CollectableItem ScriptableObject that was passed into this method, I instantiate
            // the prefab in the game world. I drop it slightly in front of (2 units) and above the player.
            // NOTE: The prefab is is initially inactive to prevent the newly instantiated objects Awake() and Start() methods 
            // from running before I have had a chance to set it up properly. I activate it after I've set it up.
            Vector3 dropPosition = transform.position + transform.up + transform.forward * 2;
            GameObject droppedObject = Instantiate(itemToDrop.prefab, dropPosition, Quaternion.identity);

            // The newly instantiated object needs to have its CollectableController's collectableItem property set 
            // to the item we are dropping.
            droppedObject.GetComponent<CollectableController>().theItemsSO = itemToDrop;

            // Let's also name the dropped object appropriately
            droppedObject.name = "Item - " + itemToDrop.displayName;

            // Now that I've finished setting it up, we can activate the dropped object
            droppedObject.SetActive(true);
        }
    }

    // This method is a handly method to drop the currently active item in the player's inventory. It pretty much just
    // calls DropItem() with the active item.
    public void DropActiveItem()
    {
        InventoryItem activeItem = playerInventory.GetActiveItem();
        if (activeItem != null)
        {
            DropItem(activeItem.theItemsSO);
        }
    }
}
