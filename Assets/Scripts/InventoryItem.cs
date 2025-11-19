using System;
using UnityEngine;

// Making a class serializable allows Unity to display it in the Inspector and serialize it properly. Serializing
// a class, means saving it to disk so that its data persists between play sessions. By default, Unity can only 
// serialize certain types of data (for example, basic types (e.g. int, float, string), arrays, and classes marked 
// with [Serializable]). Marking a class with [Serializable] does not automatically save its data to disk; it simply tells
// Unity that it can serialize instances of this class when you use the appropriate serialization mechanisms.
//
// However, if a class is marked as [Serializable], and the class is used as a field in a MonoBehaviour, then Unity
// will display the class's fields in the Inspector. I have a List of InventoryItem objects in my InventoryManager script,
// and because InventoryItem is marked as [Serializable], Unity can display the list and its contents in the Inspector. This
// is super handy for debugging and testing purposes.
[Serializable]
public class InventoryItem
{
    public CollectableItem theItemsSO;
    public int quantity;
}
