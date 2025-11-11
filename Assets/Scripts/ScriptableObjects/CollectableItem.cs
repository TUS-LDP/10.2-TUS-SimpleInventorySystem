using UnityEngine;

[CreateAssetMenu(fileName = "CollectableItem", menuName = "Scriptable Objects/CollectableItem")]
public class CollectableItem : ScriptableObject
{
    public string displayName;
    public InventoryType itemType;
    public int value;
    public Sprite icon;
    public Color color = Color.white;

}
